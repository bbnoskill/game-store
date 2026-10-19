using Gamestore.BLL.DTOs.Game.Filtering;
using Gamestore.BLL.Filtering;
using Gamestore.BLL.Services;
using Gamestore.DAL.Entities;
using Gamestore.DAL.UnitOfWork;
using Gamestore.MongoDAL.Entities;
using Gamestore.MongoDAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Gamestore.BLL.Tests.Services;

public class SqlCatalogTests
{
    [Fact]
    public async Task SqlOnlyCatalogExcludesMongoBeforePaginationAndDetailLookup()
    {
        var games = new[]
        {
            new Game { Id = Guid.NewGuid(), Key = "first", Name = "First", Price = 10 },
            new Game { Id = Guid.NewGuid(), Key = "second", Name = "Second", Price = 20 },
        };
        var unit = new Mock<IUnitOfWork>();
        unit.Setup(u => u.Games.GetQueryable()).Returns(games.AsQueryable());
        unit.Setup(u => u.Games.GetAllAsync()).ReturnsAsync(games);
        var mongo = new Mock<IMongoProductRepository>();
        mongo.Setup(m => m.GetAllQueryable()).Returns(new[] { new MongoProduct { ProductName = "Food" } }.AsQueryable());
        var service = new GameService(unit.Object, Mock.Of<ILogger<GameService>>(), new GameFilterPipeline([]), mongo.Object);

        var result = await service.GetFilteredAsync(new GameFilterDto { PageSize = "1", Page = 2, SortBy = "Price ASC" }, includeMongo: false);

        Assert.Equal(2, result.TotalPages);
        Assert.Equal("second", Assert.Single(result.Games).Key);
        Assert.Equal(2, (await service.GetAllAsync(includeMongo: false)).Count());
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetByKeyAsync("food", includeMongo: false));
        mongo.Verify(m => m.GetAllQueryable(), Times.Never);
        mongo.Verify(m => m.GetByGameStoreKeyAsync(It.IsAny<string>()), Times.Never);
        Assert.Equal(3, (await service.GetAllAsync()).Count());
    }

    [Fact]
    public async Task SqlOnlyFiltersExcludeMongoCategoriesAndSuppliers()
    {
        var unit = new Mock<IUnitOfWork>();
        unit.Setup(u => u.Genres.GetAllAsync()).ReturnsAsync([new Genre { Id = Guid.NewGuid(), Name = "Action" }]);
        unit.Setup(u => u.Publishers.GetAllAsync()).ReturnsAsync([new Publisher { Id = Guid.NewGuid(), CompanyName = "Studio" }]);
        var categories = new Mock<IMongoCategoryRepository>();
        categories.Setup(m => m.GetAllQueryable()).Returns(new[] { new MongoCategory { CategoryName = "Food" } }.AsQueryable());
        var suppliers = new Mock<IMongoSupplierRepository>();
        suppliers.Setup(m => m.GetAllQueryable()).Returns(new[] { new MongoSupplier { CompanyName = "Supplier" } }.AsQueryable());
        var genres = new GenreService(unit.Object, Mock.Of<ILogger<GameService>>(), categories.Object);
        var publishers = new PublisherService(unit.Object, Mock.Of<ILogger<GameService>>(), suppliers.Object);

        Assert.Equal("Action", Assert.Single(await genres.GetAllAsync(includeMongo: false)).Name);
        Assert.Equal("Studio", Assert.Single(await publishers.GetAllAsync(includeMongo: false)).CompanyName);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => genres.GetByIdAsync(Guid.NewGuid(), includeMongo: false));
        await Assert.ThrowsAsync<KeyNotFoundException>(() => publishers.GetByNameAsync("Supplier", includeMongo: false));
        categories.Verify(m => m.GetAllQueryable(), Times.Never);
        suppliers.Verify(m => m.GetAllQueryable(), Times.Never);
        suppliers.Verify(m => m.GetByCompanyNameAsync(It.IsAny<string>()), Times.Never);
        Assert.Equal(2, (await genres.GetAllAsync()).Count());
        Assert.Equal(2, (await publishers.GetAllAsync()).Count());
    }
}
