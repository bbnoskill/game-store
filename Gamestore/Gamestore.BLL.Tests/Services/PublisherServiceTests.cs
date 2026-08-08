using Gamestore.BLL.DTOs.Publisher;
using Gamestore.BLL.Services;
using Gamestore.DAL.Entities;
using Gamestore.DAL.Repositories.Interfaces;
using Gamestore.DAL.UnitOfWork;
using Gamestore.MongoDAL.Entities;
using Gamestore.MongoDAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Gamestore.BLL.Tests.Services;

public class PublisherServiceTests
{
    private readonly Mock<IPublisherRepository> _publisherRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IMongoSupplierRepository> _mongoSupplierRepo = new();
    private readonly PublisherService _service;
    private readonly Mock<ILogger<GameService>> _loggerMock = new();

    public PublisherServiceTests()
    {
        _uow.Setup(u => u.Publishers).Returns(_publisherRepo.Object);
        _mongoSupplierRepo.Setup(m => m.GetAllQueryable()).Returns(new List<MongoSupplier>().AsQueryable());
        _service = new PublisherService(_uow.Object, _loggerMock.Object, _mongoSupplierRepo.Object);
    }

    [Fact]
    public async Task GetByNameAsyncReturnsDtoWhenFound()
    {
        var publisher = new Publisher { Id = Guid.NewGuid(), CompanyName = "EA", Description = "Test", HomePage = "url" };
        _publisherRepo.Setup(r => r.GetByCompanyNameAsync("EA")).ReturnsAsync(publisher);

        var result = await _service.GetByNameAsync("EA");

        Assert.NotNull(result);
        Assert.Equal("EA", result.CompanyName);
    }

    [Fact]
    public async Task GetByNameAsyncThrowsWhenNotFound()
    {
        _publisherRepo.Setup(r => r.GetByCompanyNameAsync(It.IsAny<string>())).ReturnsAsync((Publisher?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByNameAsync("miss"));
    }

    [Fact]
    public async Task GetByGameKeyAsyncReturnsDtoWhenFound()
    {
        var publisher = new Publisher { Id = Guid.NewGuid(), CompanyName = "MS", Description = "Test" };
        _publisherRepo.Setup(r => r.GetByGameKeyAsync("halo")).ReturnsAsync(publisher);

        var result = await _service.GetByGameKeyAsync("halo");

        Assert.NotNull(result);
        Assert.Equal("MS", result.CompanyName);
    }

    [Fact]
    public async Task GetAllAsyncReturnsAllPublishers()
    {
        var pubs = new List<Publisher> { new() { Id = Guid.NewGuid(), CompanyName = "Pub1" } };
        _publisherRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(pubs);

        var result = (await _service.GetAllAsync()).ToList();

        Assert.Single(result);
        Assert.Equal("Pub1", result[0].CompanyName);
    }

    [Fact]
    public async Task CreateAsyncAddsPublisherAndSaves()
    {
        var request = new CreatePublisherRequestDto
        {
            Publisher = new CreatePublisherDto { CompanyName = "New Co", Description = "Desc", HomePage = "url" },
        };

        await _service.CreateAsync(request);

        _publisherRepo.Verify(r => r.AddAsync(It.Is<Publisher>(p => p.CompanyName == "New Co" && p.Description == "Desc")), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsyncUpdatesAndSaves()
    {
        var id = Guid.NewGuid();
        var existing = new Publisher { Id = id, CompanyName = "Old", Description = "Old" };
        _publisherRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

        var request = new UpdatePublisherRequestDto
        {
            Publisher = new UpdatePublisherDto { Id = id, CompanyName = "New", Description = "New", HomePage = "url" },
        };

        await _service.UpdateAsync(request);

        Assert.Equal("New", existing.CompanyName);
        Assert.Equal("New", existing.Description);
        _publisherRepo.Verify(r => r.Update(existing), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsyncRemovesAndSaves()
    {
        var id = Guid.NewGuid();
        var existing = new Publisher { Id = id, CompanyName = "Old" };
        _publisherRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

        await _service.DeleteAsync(id);

        _publisherRepo.Verify(r => r.Delete(existing), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
