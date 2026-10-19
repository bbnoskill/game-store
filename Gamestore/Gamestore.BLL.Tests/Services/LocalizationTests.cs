using Gamestore.BLL.DTOs.Game;
using Gamestore.BLL.DTOs.Game.Filtering;
using Gamestore.BLL.Filtering;
using Gamestore.BLL.Filtering.Implementations;
using Gamestore.BLL.Services;
using Gamestore.DAL.Data;
using Gamestore.DAL.Entities;
using Gamestore.DAL.Repositories.Interfaces;
using Gamestore.DAL.UnitOfWork;
using Gamestore.MongoDAL.Entities;
using Gamestore.MongoDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Gamestore.BLL.Tests.Services;

public class LocalizationTests
{
    [Theory]
    [InlineData(null, "en")]
    [InlineData("", "en")]
    [InlineData("de-DE", "en")]
    [InlineData("uk-UA", "uk")]
    [InlineData("UA", "uk")]
    [InlineData("JA-jp", "ja")]
    [InlineData("en;q=0.1,uk;q=0.9", "uk")]
    [InlineData("uk;q=0,ja;q=0.5", "ja")]
    [InlineData("uk;q=broken,ja", "ja")]
    [InlineData("uk;q=1.1,ja;q=0.5", "ja")]
    [InlineData("ja;q=0.5,uk;q=0.5", "ja")]
    [InlineData("*", "en")]
    public void HeaderResolvesSupportedLanguage(string? header, string expected)
    {
        Assert.Equal(expected, new LanguageService().NormalizeLanguageCode(header));
    }

    [Theory]
    [InlineData("uk", "Українська гра")]
    [InlineData("ja", "English game")]
    [InlineData("en", "English game")]
    public async Task GameReadsUseTranslationAndEnglishFallbackWithoutMutatingSource(string language, string expected)
    {
        var game = CreateGame();
        var repository = new Mock<IGameRepository>();
        repository.Setup(r => r.GetByIdAsync(game.Id)).ReturnsAsync(game);
        repository.Setup(r => r.GetByKeyAsync(game.Key)).ReturnsAsync(game);
        repository.Setup(r => r.GetAllAsync()).ReturnsAsync([game]);
        repository.Setup(r => r.GetByGenreAsync(game.Id)).ReturnsAsync([game]);
        repository.Setup(r => r.GetByPlatformAsync(game.Id)).ReturnsAsync([game]);
        repository.Setup(r => r.GetByPublisherAsync("Publisher")).ReturnsAsync([game]);
        repository.Setup(r => r.GetQueryable()).Returns(new[] { game }.AsQueryable());
        var service = CreateService(repository);

        Assert.Equal(expected, (await service.GetByIdAsync(game.Id, language))!.Name);
        Assert.Equal(expected, (await service.GetByKeyAsync(game.Key, language))!.Name);
        Assert.Equal(expected, Assert.Single(await service.GetAllAsync(language)).Name);
        Assert.Equal(expected, Assert.Single(await service.GetByGenreAsync(game.Id, language)).Name);
        Assert.Equal(expected, Assert.Single(await service.GetByPlatformAsync(game.Id, language)).Name);
        Assert.Equal(expected, Assert.Single(await service.GetByPublisherAsync("Publisher", language)).Name);
        Assert.Equal(expected, Assert.Single((await service.GetFilteredAsync(new GameFilterDto(), language)).Games).Name);
        Assert.Equal("Legacy English", game.Name);
        Assert.Equal("English description", (await service.GetByIdAsync(game.Id, language))!.Description);
    }

    [Fact]
    public async Task GameDetailsUseSqlFunctionResult()
    {
        var game = CreateGame();
        var repository = new Mock<IGameRepository>();
        repository.Setup(r => r.GetByKeyAsync(game.Key)).ReturnsAsync(game);
        repository.Setup(r => r.GetLocalizedInfoAsync(game.Key, "ja"))
            .ReturnsAsync(new LocalizedGameInfo { Id = game.Id, Key = game.Key, Name = "ゲーム", Price = 42 });

        var result = await CreateService(repository).GetByKeyAsync(game.Key, "ja");

        Assert.Equal("ゲーム", result!.Name);
        Assert.Equal(42, result.Price);
        repository.Verify(r => r.GetLocalizedInfoAsync(game.Key, "ja"), Times.Once);
    }

    [Fact]
    public async Task UpdateTranslationPreservesEnglishAndDoesNotDuplicateRows()
    {
        var game = CreateGame();
        var repository = new Mock<IGameRepository>();
        repository.Setup(r => r.GetByIdAsync(game.Id)).ReturnsAsync(game);
        var service = CreateService(repository);
        var request = new UpdateGameRequestDto
        {
            Game = new UpdateGameDto { Id = game.Id, Key = game.Key, Name = "Нова назва", Description = "Новий опис" },
            Genres = [],
            Platforms = [],
        };

        await service.UpdateAsync(request, "uk");
        await service.UpdateAsync(request, "uk");

        Assert.Equal("Legacy English", game.Name);
        Assert.Equal("English game", game.Localizations.Single(l => l.LanguageCode == "en").Name);
        Assert.Equal("Нова назва", game.Localizations.Single(l => l.LanguageCode == "uk").Name);
        Assert.Equal(2, game.Localizations.Count);

        request.Game.Name = "Updated English";
        await service.UpdateAsync(request);
        Assert.Equal("Updated English", (await service.GetByIdAsync(game.Id, "ja"))!.Name);
        Assert.Equal("Нова назва", (await service.GetByIdAsync(game.Id, "uk"))!.Name);
    }

    [Fact]
    public void NameFilterUsesSelectedLanguageAndFallback()
    {
        var games = new[] { CreateGame() }.AsQueryable();
        var step = new NameFilterStep();
        Assert.Single(step.Execute(games, new GameFilterDto { Name = "Українська", LanguageCode = "uk" }));
        Assert.Single(step.Execute(games, new GameFilterDto { Name = "English game", LanguageCode = "ja" }));
        Assert.Empty(step.Execute(games, new GameFilterDto { Name = "Українська", LanguageCode = "en" }));
    }

    [Fact]
    public void OtherEntitiesKeepIndependentTranslationsAndEnglishVersions()
    {
        var genre = new Genre { Id = Guid.NewGuid(), Name = "Action" };
        var platform = new Platform { Id = Guid.NewGuid(), Type = "Desktop" };
        var publisher = new Publisher { Id = Guid.NewGuid(), CompanyName = "Publisher", Description = "English" };
        EntityLocalization.Update(genre, "uk", "Екшн");
        EntityLocalization.Update(platform, "ja", "デスクトップ");
        EntityLocalization.Update(publisher, "uk", "Видавець", "Опис");
        EntityLocalization.Update(publisher, "uk", "Видавець", "Новий опис");

        Assert.Equal("Action", genre.Name);
        Assert.Equal("Екшн", Assert.Single(genre.Localizations).Name);
        Assert.Equal("Desktop", platform.Type);
        Assert.Equal("デスクトップ", Assert.Single(platform.Localizations).Type);
        Assert.Equal("English", publisher.Description);
        Assert.Equal("Новий опис", Assert.Single(publisher.Localizations).Description);
    }

    [Theory]
    [InlineData(typeof(Game), typeof(GameLocalization))]
    [InlineData(typeof(Genre), typeof(GenreLocalization))]
    [InlineData(typeof(Platform), typeof(PlatformLocalization))]
    [InlineData(typeof(Publisher), typeof(PublisherLocalization))]
    public void DeletingEntityMarksEveryLoadedLocalizationForDeletion(Type entityType, Type translationType)
    {
        using var context = new GamestoreContext(new DbContextOptionsBuilder<GamestoreContext>()
            .UseSqlServer("Server=localhost;Database=ModelOnly;Integrated Security=True;TrustServerCertificate=True").Options);
        var entity = Activator.CreateInstance(entityType)!;
        var id = Guid.NewGuid();
        entityType.GetProperty("Id")!.SetValue(entity, id);
        context.Attach(entity);
        foreach (var language in new[] { "en", "uk", "ja" })
        {
            var translation = Activator.CreateInstance(translationType)!;
            translationType.GetProperty("Id")!.SetValue(translation, Guid.NewGuid());
            translationType.GetProperty(entityType.Name + "Id")!.SetValue(translation, id);
            translationType.GetProperty("LanguageCode")!.SetValue(translation, language);
            context.Attach(translation);
        }

        context.Remove(entity);

        Assert.Equal(3, context.ChangeTracker.Entries().Count(e => e.Entity.GetType() == translationType && e.State == EntityState.Deleted));
        var relationship = context.Model.FindEntityType(translationType)!.GetForeignKeys().Single();
        Assert.Equal(DeleteBehavior.Cascade, relationship.DeleteBehavior);
        Assert.True(context.Model.FindEntityType(entityType)!.FindNavigation("Localizations")!.IsEagerLoaded);
    }

    private static Game CreateGame() => new()
    {
        Id = Guid.NewGuid(),
        Key = "stable-key",
        Name = "Legacy English",
        Localizations =
        [
            new() { LanguageCode = "en", Name = "English game", Description = "English description" },
            new() { LanguageCode = "uk", Name = "Українська гра" },
        ],
    };

    private static GameService CreateService(Mock<IGameRepository> repository)
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.Games).Returns(repository.Object);
        var mongo = new Mock<IMongoProductRepository>();
        mongo.Setup(m => m.GetAllQueryable()).Returns(Array.Empty<MongoProduct>().AsQueryable());
        return new GameService(unitOfWork.Object, Mock.Of<ILogger<GameService>>(), new GameFilterPipeline([new NameFilterStep()]), mongo.Object);
    }
}
