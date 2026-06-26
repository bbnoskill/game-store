using Gamestore.BLL.DTOs.Genre;
using Gamestore.BLL.Services;
using Gamestore.DAL.Entities;
using Gamestore.DAL.Repositories.Interfaces;
using Gamestore.DAL.UnitOfWork;
using Moq;

namespace Gamestore.BLL.Tests.Services;

public class GenreServiceTests
{
    private readonly Mock<IGenreRepository> _genreRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly GenreService _service;

    public GenreServiceTests()
    {
        _uow.Setup(u => u.Genres).Returns(_genreRepo.Object);
        _service = new GenreService(_uow.Object);
    }

    [Fact]
    public async Task GetByIdAsyncReturnsDtoWhenFound()
    {
        var id = Guid.NewGuid();
        var genre = new Genre { Id = id, Name = "RPG", ParentGenreId = null };
        _genreRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(genre);

        var result = await _service.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal("RPG", result.Name);
    }

    [Fact]
    public async Task GetByIdAsyncReturnsNullWhenNotFound()
    {
        _genreRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Genre?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsyncReturnsMappedDtos()
    {
        var genres = new List<Genre>
        {
            new() { Id = Guid.NewGuid(), Name = "Action" },
            new() { Id = Guid.NewGuid(), Name = "RPG" },
        };
        _genreRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(genres);

        var result = (await _service.GetAllAsync()).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByGameKeyAsyncReturnsMappedDtos()
    {
        var genres = new List<Genre> { new() { Id = Guid.NewGuid(), Name = "Action" } };
        _genreRepo.Setup(r => r.GetByGameKeyAsync("halo")).ReturnsAsync(genres);

        var result = (await _service.GetByGameKeyAsync("halo")).ToList();

        Assert.Single(result);
        _genreRepo.Verify(r => r.GetByGameKeyAsync("halo"), Times.Once);
    }

    [Fact]
    public async Task GetByParentIdAsyncReturnsMappedDtos()
    {
        var parentId = Guid.NewGuid();
        var genres = new List<Genre>
        {
            new() { Id = Guid.NewGuid(), Name = "FPS", ParentGenreId = parentId },
        };
        _genreRepo.Setup(r => r.GetByParentIdAsync(parentId)).ReturnsAsync(genres);

        var result = (await _service.GetByParentIdAsync(parentId)).ToList();

        Assert.Single(result);
        Assert.Equal(parentId, result[0].ParentGenreId);
    }

    [Fact]
    public async Task CreateAsyncAddsGenreAndSaves()
    {
        var request = new CreateGenreRequestDto
        {
            Genre = new CreateGenreDto { Name = "Survival", ParentGenreId = null },
        };

        await _service.CreateAsync(request);

        _genreRepo.Verify(r => r.AddAsync(It.Is<Genre>(g => g.Name == "Survival")), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsyncUpdatesGenreAndSaves()
    {
        var id = Guid.NewGuid();
        var existing = new Genre { Id = id, Name = "Old", ParentGenreId = null };
        _genreRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

        var request = new UpdateGenreRequestDto
        {
            Genre = new UpdateGenreDto { Id = id, Name = "New", ParentGenreId = Guid.NewGuid() },
        };

        await _service.UpdateAsync(request);

        Assert.Equal("New", existing.Name);
        Assert.NotNull(existing.ParentGenreId);
        _genreRepo.Verify(r => r.Update(existing), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsyncThrowsWhenNotFound()
    {
        _genreRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Genre?)null);

        var request = new UpdateGenreRequestDto
        {
            Genre = new UpdateGenreDto { Id = Guid.NewGuid(), Name = "X", ParentGenreId = null },
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(request));
    }

    [Fact]
    public async Task DeleteAsyncDeletesGenreWhenFound()
    {
        var id = Guid.NewGuid();
        var genre = new Genre { Id = id, Name = "X" };
        _genreRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(genre);

        await _service.DeleteAsync(id);

        _genreRepo.Verify(r => r.Delete(genre), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsyncThrowsWhenNotFound()
    {
        _genreRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Genre?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(Guid.NewGuid()));
    }
}