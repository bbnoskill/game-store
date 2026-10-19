using Gamestore.BLL.Services;
using Gamestore.DAL.Data;
using Gamestore.DAL.Entities;
using Gamestore.DAL.Repositories.Implementations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.BLL.Tests.Services;

public class LocalizationSqlTests
{
    [SqlServerFact]
    public async Task MigrationsFunctionFallbackAndCascadingDeletesWorkInSqlServer()
    {
        var connection = new SqlConnectionStringBuilder(Environment.GetEnvironmentVariable("GAMESTORE_TEST_SQL_CONNECTION"))
        {
            InitialCatalog = "GameStore_LocalizationTest_" + Guid.NewGuid().ToString("N"),
            ConnectTimeout = 10,
        };
        await using var context = new GamestoreContext(new DbContextOptionsBuilder<GamestoreContext>()
            .UseSqlServer(connection.ConnectionString).Options);
        try
        {
            await context.Database.MigrateAsync();
            var publisher = new Publisher { Id = Guid.NewGuid(), CompanyName = "Test publisher" };
            var genre = new Genre { Id = Guid.NewGuid(), Name = "Test genre" };
            var platform = new Platform { Id = Guid.NewGuid(), Type = "Test platform" };
            var game = new Game { Id = Guid.NewGuid(), Name = "Original", Key = new string('g', 140), Publisher = publisher };
            foreach (var language in new[] { "en", "uk", "ja" })
            {
                publisher.Localizations.Add(new PublisherLocalization { Id = Guid.NewGuid(), LanguageCode = language, CompanyName = language });
                genre.Localizations.Add(new GenreLocalization { Id = Guid.NewGuid(), LanguageCode = language, Name = language });
                platform.Localizations.Add(new PlatformLocalization { Id = Guid.NewGuid(), LanguageCode = language, Type = language });
                game.Localizations.Add(new GameLocalization { Id = Guid.NewGuid(), LanguageCode = language, Name = language, Description = language == "en" ? "English description" : null });
            }

            context.AddRange(publisher, genre, platform, game);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
            var repository = new GameRepository(context);
            Assert.Equal("uk", (await repository.GetLocalizedInfoAsync(game.Key, "uk"))!.Name);
            Assert.Equal("ja", (await repository.GetLocalizedInfoAsync(game.Key, "ja"))!.Name);
            Assert.Equal("en", (await repository.GetLocalizedInfoAsync(game.Key, "fr"))!.Name);
            Assert.Equal("English description", (await repository.GetLocalizedInfoAsync(game.Key, "uk"))!.Description);
            Assert.Null(await repository.GetLocalizedInfoAsync("not-found", "en"));
            Assert.Equal(3, (await repository.GetByIdAsync(game.Id))!.Localizations.Count);
            var trackedGame = (await repository.GetByIdAsync(game.Id))!;
            var japanese = trackedGame.Localizations.Single(l => l.LanguageCode == "ja");
            context.GameLocalizations.Remove(japanese);
            await context.SaveChangesAsync();
            EntityLocalization.Update(trackedGame, "ja", "日本語の更新", "説明");
            repository.Update(trackedGame);
            await context.SaveChangesAsync();
            Assert.Equal("日本語の更新", (await repository.GetLocalizedInfoAsync(game.Key, "ja"))!.Name);
            Assert.Equal("en", (await repository.GetLocalizedInfoAsync(game.Key, "en"))!.Name);
            trackedGame.ViewCount++;
            repository.Update(trackedGame);
            Assert.All(context.ChangeTracker.Entries<GameLocalization>(), entry => Assert.Equal(EntityState.Unchanged, entry.State));
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            // Leave dependent translations unloaded: SQL must cascade every language itself.
            context.Games.Remove(new Game { Id = game.Id });
            context.Genres.Remove(new Genre { Id = genre.Id });
            context.Platforms.Remove(new Platform { Id = platform.Id });
            await context.SaveChangesAsync();
            context.Publishers.Remove(new Publisher { Id = publisher.Id });
            await context.SaveChangesAsync();
            Assert.False(await context.GameLocalizations.AnyAsync(l => l.GameId == game.Id));
            Assert.False(await context.GenreLocalizations.AnyAsync(l => l.GenreId == genre.Id));
            Assert.False(await context.PlatformLocalizations.AnyAsync(l => l.PlatformId == platform.Id));
            Assert.False(await context.PublisherLocalizations.AnyAsync(l => l.PublisherId == publisher.Id));
        }
        finally
        {
            // This connection always targets the unique disposable database created above.
            await context.Database.EnsureDeletedAsync();
        }
    }
}
