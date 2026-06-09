using Gamestore.BLL.DTOs.Platform;
using Gamestore.BLL.Services;
using Gamestore.DAL.Entities;
using Gamestore.DAL.Repositories.Interfaces;
using Gamestore.DAL.UnitOfWork;
using Moq;

namespace Gamestore.BLL.Tests.Services;

public class PlatformServiceTests
{
    private readonly Mock<IPlatformRepository> _platformRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly PlatformService _service;

    public PlatformServiceTests()
    {
        _uow.Setup(u => u.Platforms).Returns(_platformRepo.Object);
        _service = new PlatformService(_uow.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WhenFound()
    {
        var id = Guid.NewGuid();
        var platform = new Platform { Id = id, Type = "PC" };
        _platformRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(platform);

        var result = await _service.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal("PC", result!.Type);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        _platformRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Platform?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedDtos()
    {
        var platforms = new List<Platform>
        {
            new() { Id = Guid.NewGuid(), Type = "PC" },
            new() { Id = Guid.NewGuid(), Type = "Console" },
        };
        _platformRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(platforms);

        var result = (await _service.GetAllAsync()).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByGameKeyAsync_ReturnsMappedDtos()
    {
        var platforms = new List<Platform> { new() { Id = Guid.NewGuid(), Type = "PC" } };
        _platformRepo.Setup(r => r.GetByGameKeyAsync("halo")).ReturnsAsync(platforms);

        var result = (await _service.GetByGameKeyAsync("halo")).ToList();

        Assert.Single(result);
        _platformRepo.Verify(r => r.GetByGameKeyAsync("halo"), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_AddsPlatformAndSaves()
    {
        var request = new CreatePlatformRequestDto
        {
            Platform = new CreatePlatformDto { Type = "VR" },
        };

        await _service.CreateAsync(request);

        _platformRepo.Verify(r => r.AddAsync(It.Is<Platform>(p => p.Type == "VR")), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesPlatformAndSaves()
    {
        var id = Guid.NewGuid();
        var existing = new Platform { Id = id, Type = "Old" };
        _platformRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

        var request = new UpdatePlatformRequestDto
        {
            Platform = new UpdatePlatformDto { Id = id, Type = "New" },
        };

        await _service.UpdateAsync(request);

        Assert.Equal("New", existing.Type);
        _platformRepo.Verify(r => r.UpdateAsync(existing), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Throws_WhenNotFound()
    {
        _platformRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Platform?)null);

        var request = new UpdatePlatformRequestDto
        {
            Platform = new UpdatePlatformDto { Id = Guid.NewGuid(), Type = "X" },
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(request));
    }

    [Fact]
    public async Task DeleteAsync_DeletesPlatform_WhenFound()
    {
        var id = Guid.NewGuid();
        var platform = new Platform { Id = id, Type = "PC" };
        _platformRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(platform);

        await _service.DeleteAsync(id);

        _platformRepo.Verify(r => r.DeleteAsync(platform), Times.Once);

        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Throws_WhenNotFound()
    {
        _platformRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Platform?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(Guid.NewGuid()));
    }
}