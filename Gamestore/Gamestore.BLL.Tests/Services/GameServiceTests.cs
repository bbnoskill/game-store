using Gamestore.BLL.DTOs.Game;
using Gamestore.BLL.Filtering;
using Gamestore.BLL.Services;
using Gamestore.DAL.Entities;
using Gamestore.DAL.Repositories.Interfaces;
using Gamestore.DAL.UnitOfWork;
using Microsoft.Extensions.Logging;
using Moq;

namespace Gamestore.BLL.Tests.Services;

public class GameServiceTests
{
    private readonly Mock<IGameRepository> _gameRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly GameService _service;
    private readonly Mock<ILogger<GameService>> _loggerMock = new();
    private readonly GameFilterPipeline _pipeline = new([]);

    public GameServiceTests()
    {
        _uow.Setup(u => u.Games).Returns(_gameRepo.Object);
        _service = new GameService(_uow.Object, _loggerMock.Object, _pipeline);
    }

    [Fact]
    public async Task GetByKeyAsyncReturnsDtoWhenFound()
    {
        var game = new Game { Id = Guid.NewGuid(), Name = "Halo", Key = "halo", Description = "FPS" };
        _gameRepo.Setup(r => r.GetByKeyAsync("halo")).ReturnsAsync(game);

        var result = await _service.GetByKeyAsync("halo");

        Assert.NotNull(result);
        Assert.Equal(game.Id, result.Id);
        Assert.Equal("Halo", result.Name);
        Assert.Equal("halo", result.Key);
    }

    [Fact]
    public async Task GetByKeyAsyncReturnsNullWhenNotFound()
    {
        _gameRepo.Setup(r => r.GetByKeyAsync(It.IsAny<string>())).ReturnsAsync((Game?)null);

        var result = await _service.GetByKeyAsync("missing");

        Assert.Null(result);
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
    public async Task GetByIdAsyncReturnsNullWhenNotFound()
    {
        _gameRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Game?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsyncReturnsAllMappedDtos()
    {
        var games = new List<Game>
        {
            new() { Id = Guid.NewGuid(), Name = "G1", Key = "g1", Description = string.Empty },
            new() { Id = Guid.NewGuid(), Name = "G2", Key = "g2", Description = string.Empty },
        };
        _gameRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(games);

        var result = (await _service.GetAllAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, g => g.Name == "G1");
        Assert.Contains(result, g => g.Name == "G2");
    }

    [Fact]
    public async Task GetByGenreAsyncReturnsMappedDtos()
    {
        var genreId = Guid.NewGuid();
        var games = new List<Game> { new() { Id = Guid.NewGuid(), Name = "X", Key = "x", Description = string.Empty } };
        _gameRepo.Setup(r => r.GetByGenreAsync(genreId)).ReturnsAsync(games);

        var result = (await _service.GetByGenreAsync(genreId)).ToList();

        Assert.Single(result);
        _gameRepo.Verify(r => r.GetByGenreAsync(genreId), Times.Once);
    }

    [Fact]
    public async Task GetByPlatformAsyncReturnsMappedDtos()
    {
        var platformId = Guid.NewGuid();
        var games = new List<Game> { new() { Id = Guid.NewGuid(), Name = "Y", Key = "y", Description = string.Empty } };
        _gameRepo.Setup(r => r.GetByPlatformAsync(platformId)).ReturnsAsync(games);

        var result = (await _service.GetByPlatformAsync(platformId)).ToList();

        Assert.Single(result);
        _gameRepo.Verify(r => r.GetByPlatformAsync(platformId), Times.Once);
    }

    [Fact]
    public async Task CreateAsyncAddsGameAndSaves()
    {
        var request = new CreateGameRequestDto
        {
            Game = new CreateGameDto { Name = "New Game", Key = "new-game", Description = "desc" },
            Genres = [Guid.NewGuid()],
            Platforms = [Guid.NewGuid()],
        };

        await _service.CreateAsync(request);

        _gameRepo.Verify(
            r => r.AddAsync(It.Is<Game>(g =>
            g.Name == "New Game" &&
            g.Key == "new-game" &&
            g.GameGenres.Count == 1 &&
            g.GamePlatforms.Count == 1)),
            Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsyncGeneratesKeyWhenKeyEmpty()
    {
        var request = new CreateGameRequestDto
        {
            Game = new CreateGameDto { Name = "My Cool Game", Key = string.Empty, Description = string.Empty },
            Genres = [],
            Platforms = [],
        };

        Game? captured = null;
        _gameRepo.Setup(r => r.AddAsync(It.IsAny<Game>()))
                 .Callback<Game>(g => captured = g)
                 .Returns(Task.CompletedTask);

        await _service.CreateAsync(request);

        Assert.NotNull(captured);

        Assert.Equal("my-cool-game", captured!.Key);
    }

    [Fact]
    public async Task UpdateAsyncUpdatesGameAndSaves()
    {
        var id = Guid.NewGuid();
        var existing = new Game { Id = id, Name = "Old", Key = "old", Description = "old" };
        _gameRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

        var request = new UpdateGameRequestDto
        {
            Game = new UpdateGameDto { Id = id, Name = "Updated", Key = "updated", Description = "new" },
            Genres = [Guid.NewGuid()],
            Platforms = [Guid.NewGuid()],
        };

        await _service.UpdateAsync(request);

        Assert.Equal("Updated", existing.Name);
        Assert.Equal("updated", existing.Key);
        Assert.Equal("new", existing.Description);
        _gameRepo.Verify(r => r.Update(existing), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsyncGeneratesKeyWhenKeyWhitespace()
    {
        var id = Guid.NewGuid();
        var existing = new Game { Id = id, Name = "Old", Key = "old", Description = string.Empty };
        _gameRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

        var request = new UpdateGameRequestDto
        {
            Game = new UpdateGameDto { Id = id, Name = "Cool Game", Key = "   ", Description = string.Empty },
            Genres = [],
            Platforms = [],
        };

        await _service.UpdateAsync(request);

        Assert.Equal("cool-game", existing.Key);
    }

    [Fact]
    public async Task UpdateAsyncThrowsWhenGameNotFound()
    {
        _gameRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Game?)null);

        var request = new UpdateGameRequestDto
        {
            Game = new UpdateGameDto { Id = Guid.NewGuid(), Name = "x", Key = "x", Description = string.Empty },
            Genres = [],
            Platforms = [],
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(request));
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

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync("nope"));
    }

    [Fact]
    public async Task GetTotalCountAsyncReturnsCountFromRepository()
    {
        _gameRepo.Setup(r => r.GetCountAsync()).ReturnsAsync(42);

        var result = await _service.GetTotalCountAsync();

        Assert.Equal(42, result);
        _gameRepo.Verify(r => r.GetCountAsync(), Times.Once);
    }
}