using Gamestore.BLL.DTOs.Game;
using Gamestore.BLL.DTOs.Game.Filtering;
using Gamestore.BLL.Filtering;
using Gamestore.BLL.Filtering.Implementations;
using Gamestore.BLL.Services;
using Gamestore.DAL.Entities;
using Gamestore.DAL.Repositories.Interfaces;
using Gamestore.DAL.UnitOfWork;
using Gamestore.MongoDAL.Entities;
using Gamestore.MongoDAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Gamestore.BLL.Tests.Services;

public class GameServiceTests
{
    private readonly Mock<IGameRepository> _gameRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IMongoProductRepository> _mongoProductRepo = new();
    private readonly GameFilterPipeline _pipeline;
    private readonly GameService _service;
    private readonly Mock<ILogger<GameService>> _loggerMock = new();

    public GameServiceTests()
    {
        _uow.Setup(u => u.Games).Returns(_gameRepo.Object);
        _mongoProductRepo.Setup(m => m.GetAllQueryable()).Returns(new List<MongoProduct>().AsQueryable());

        _pipeline = new GameFilterPipeline([
            new NameFilterStep(),
            new PriceFilterStep(),
            new GenreFilterStep(),
            new PlatformFilterStep(),
            new PublisherFilterStep(),
            new PublishDateFilterStep(),
            new SortStep(),
        ]);

        _service = new GameService(_uow.Object, _loggerMock.Object, _pipeline, _mongoProductRepo.Object);
    }

    [Fact]
    public async Task GetByKeyAsyncReturnsDtoWhenFound()
    {
        var game = new Game { Id = Guid.NewGuid(), Name = "Halo", Key = "halo", Description = string.Empty };
        _gameRepo.Setup(r => r.GetByKeyAsync("halo")).ReturnsAsync(game);

        var result = await _service.GetByKeyAsync("halo");

        Assert.NotNull(result);
        Assert.Equal("Halo", result.Name);
        Assert.Equal("halo", result.Key);
    }

    [Fact]
    public async Task GetByKeyAsyncThrowsWhenNotFound()
    {
        _gameRepo.Setup(r => r.GetByKeyAsync(It.IsAny<string>())).ReturnsAsync((Game?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByKeyAsync("missing"));
    }

    [Fact]
    public async Task GetByIdAsyncReturnsDtoWhenFound()
    {
        var id = Guid.NewGuid();
        var game = new Game { Id = id, Name = "Doom", Key = "doom", Description = string.Empty };
        _gameRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(game);

        var result = await _service.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsyncThrowsWhenNotFound()
    {
        _gameRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Game?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetAllAsyncReturnsAllGames()
    {
        var games = new List<Game>
        {
            new() { Id = Guid.NewGuid(), Name = "Halo", Key = "halo", Description = string.Empty },
            new() { Id = Guid.NewGuid(), Name = "Doom", Key = "doom", Description = string.Empty },
        };
        _gameRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(games);

        var result = (await _service.GetAllAsync()).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByGenreAsyncReturnsGamesForGenre()
    {
        var genreId = Guid.NewGuid();
        var games = new List<Game> { new() { Id = Guid.NewGuid(), Name = "RPG Game", Key = "rpg", Description = string.Empty } };
        _gameRepo.Setup(r => r.GetByGenreAsync(genreId)).ReturnsAsync(games);

        var result = (await _service.GetByGenreAsync(genreId)).ToList();

        Assert.Single(result);
        Assert.Equal("RPG Game", result[0].Name);
    }

    [Fact]
    public async Task GetByPlatformAsyncReturnsGamesForPlatform()
    {
        var platformId = Guid.NewGuid();
        var games = new List<Game> { new() { Id = Guid.NewGuid(), Name = "PC Game", Key = "pc", Description = string.Empty } };
        _gameRepo.Setup(r => r.GetByPlatformAsync(platformId)).ReturnsAsync(games);

        var result = (await _service.GetByPlatformAsync(platformId)).ToList();

        Assert.Single(result);
        Assert.Equal("PC Game", result[0].Name);
    }

    [Fact]
    public async Task GetByPublisherAsyncReturnsGamesForPublisher()
    {
        var games = new List<Game> { new() { Id = Guid.NewGuid(), Name = "EA Game", Key = "ea", Description = string.Empty } };
        _gameRepo.Setup(r => r.GetByPublisherAsync("EA")).ReturnsAsync(games);

        var result = (await _service.GetByPublisherAsync("EA")).ToList();

        Assert.Single(result);
        Assert.Equal("EA Game", result[0].Name);
    }

    [Fact]
    public async Task CreateAsyncAddsGameAndSaves()
    {
        var request = new CreateGameRequestDto
        {
            Game = new CreateGameDto { Name = "New Game", Key = "new-game", Description = "Desc", Price = 10 },
            Genres = [],
            Platforms = [],
        };

        await _service.CreateAsync(request);

        _gameRepo.Verify(r => r.AddAsync(It.Is<Game>(g => g.Name == "New Game" && g.Key == "new-game")), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsyncGeneratesKeyWhenKeyEmpty()
    {
        var request = new CreateGameRequestDto
        {
            Game = new CreateGameDto { Name = "My Cool Game", Key = string.Empty, Description = "Desc" },
            Genres = [],
            Platforms = [],
        };

        await _service.CreateAsync(request);

        _gameRepo.Verify(r => r.AddAsync(It.Is<Game>(g => g.Key == "my-cool-game")), Times.Once);
    }

    [Fact]
    public async Task UpdateAsyncUpdatesExistingGame()
    {
        var id = Guid.NewGuid();
        var existing = new Game { Id = id, Name = "Old", Key = "old", Description = "Desc" };
        _gameRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

        var request = new UpdateGameRequestDto
        {
            Game = new UpdateGameDto { Id = id, Name = "Updated", Key = "updated", Description = "New Desc" },
            Genres = [],
            Platforms = [],
        };

        await _service.UpdateAsync(request);

        Assert.Equal("Updated", existing.Name);
        Assert.Equal("updated", existing.Key);
        _gameRepo.Verify(r => r.Update(existing), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsyncGeneratesKeyWhenKeyWhitespace()
    {
        var id = Guid.NewGuid();
        var existing = new Game { Id = id, Name = "Old", Key = "old", Description = "Desc" };
        _gameRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

        var request = new UpdateGameRequestDto
        {
            Game = new UpdateGameDto { Id = id, Name = "New Name", Key = "   ", Description = "Desc" },
            Genres = [],
            Platforms = [],
        };

        await _service.UpdateAsync(request);

        Assert.Equal("new-name", existing.Key);
    }

    [Fact]
    public async Task UpdateAsyncCreatesNewGameIfNotFoundInSql()
    {
        _gameRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Game?)null);

        var request = new UpdateGameRequestDto
        {
            Game = new UpdateGameDto { Id = Guid.NewGuid(), Name = "x", Key = "x", Description = string.Empty },
            Genres = [],
            Platforms = [],
        };

        await _service.UpdateAsync(request);

        _gameRepo.Verify(r => r.AddAsync(It.IsAny<Game>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsyncDeletesGameWhenFound()
    {
        var game = new Game { Id = Guid.NewGuid(), Name = "X", Key = "x", Description = string.Empty };
        _gameRepo.Setup(r => r.GetByKeyAsync("x")).ReturnsAsync(game);

        await _service.DeleteAsync("x");

        _gameRepo.Verify(r => r.Delete(game), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsyncThrowsWhenNotFound()
    {
        _gameRepo.Setup(r => r.GetByKeyAsync(It.IsAny<string>())).ReturnsAsync((Game?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync("missing"));
    }

    [Fact]
    public async Task GetFilteredAsyncAppliesPipelineAndReturnsPagedResult()
    {
        var list = new List<Game>
        {
            new() { Id = Guid.NewGuid(), Name = "Alpha", Key = "alpha", Price = 10, Description = string.Empty },
            new() { Id = Guid.NewGuid(), Name = "Beta", Key = "beta", Price = 20, Description = string.Empty },
        }.AsQueryable();

        _gameRepo.Setup(r => r.GetQueryable()).Returns(list);

        var filter = new GameFilterDto { Name = "Alpha", Page = 1, PageSize = "10" };
        var result = await _service.GetFilteredAsync(filter);

        Assert.NotNull(result);
        Assert.Single(result.Games);
        Assert.Equal("Alpha", result.Games.First().Name);
    }
}