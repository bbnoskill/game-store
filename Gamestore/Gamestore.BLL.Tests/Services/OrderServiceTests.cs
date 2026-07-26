using System.Net;
using Gamestore.BLL.DTOs.Order;
using Gamestore.BLL.Services;
using Gamestore.DAL.Entities;
using Gamestore.DAL.Repositories.Interfaces;
using Gamestore.DAL.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace Gamestore.BLL.Tests.Services;

public class OrderServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IOrderRepository> _orderRepo = new();
    private readonly Mock<IOrderGameRepository> _orderGameRepo = new();
    private readonly Mock<IGameRepository> _gameRepo = new();
    private readonly Mock<ILogger<OrderService>> _loggerMock = new();
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock = new();
    private readonly Mock<IHttpClientFactory> _clientFactoryMock = new();
    private readonly Mock<IConfiguration> _configMock = new();

    private readonly OrderService _service;
    private readonly Guid _stubCustomerId = Guid.Parse("5aa1c97e-e6b3-497c-8e00-270e96aa0b63");

    public OrderServiceTests()
    {
        _uow.Setup(u => u.Orders).Returns(_orderRepo.Object);
        _uow.Setup(u => u.OrderGames).Returns(_orderGameRepo.Object);
        _uow.Setup(u => u.Games).Returns(_gameRepo.Object);

        var client = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };
        _clientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        var configSectionMock = new Mock<IConfigurationSection>();
        configSectionMock.Setup(c => c.Value).Returns("http://localhost:5000");
        _configMock.Setup(c => c.GetSection("Payment:MicroserviceUrl")).Returns(configSectionMock.Object);

        var validitySectionMock = new Mock<IConfigurationSection>();
        validitySectionMock.Setup(c => c.Value).Returns("3");
        _configMock.Setup(c => c.GetSection("Payment:BankInvoiceValidityDays")).Returns(validitySectionMock.Object);

        _service = new OrderService(_uow.Object, _loggerMock.Object, _clientFactoryMock.Object, _configMock.Object);
    }

    [Fact]
    public async Task AddToCartAsyncThrowsIfGameNotFound()
    {
        _gameRepo.Setup(r => r.GetByKeyAsync("x")).ReturnsAsync((Game?)null);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.AddToCartAsync("x"));
    }

    [Fact]
    public async Task AddToCartAsyncThrowsIfOutOfStock()
    {
        var game = new Game { Id = Guid.NewGuid(), UnitInStock = 0 };
        _gameRepo.Setup(r => r.GetByKeyAsync("key")).ReturnsAsync(game);

        await Assert.ThrowsAsync<ArgumentException>(() => _service.AddToCartAsync("key"));
    }

    [Fact]
    public async Task AddToCartAsyncCreatesOrderIfNotExists()
    {
        var game = new Game { Id = Guid.NewGuid(), UnitInStock = 5, Key = "key", Price = 10, Discount = 0 };
        _gameRepo.Setup(r => r.GetByKeyAsync("key")).ReturnsAsync(game);
        _orderRepo.Setup(r => r.GetOpenOrderByCustomerIdAsync(_stubCustomerId)).ReturnsAsync((Order?)null);
        _orderGameRepo.Setup(r => r.GetByOrderAndProductAsync(It.IsAny<Guid>(), game.Id)).ReturnsAsync((OrderGame?)null);

        await _service.AddToCartAsync("key");

        _orderRepo.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
        _orderGameRepo.Verify(r => r.AddAsync(It.Is<OrderGame>(og => og.Quantity == 1)), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddToCartAsyncIncrementsQuantityIfExists()
    {
        var game = new Game { Id = Guid.NewGuid(), UnitInStock = 5, Key = "key" };
        var order = new Order { Id = Guid.NewGuid() };
        var orderGame = new OrderGame { OrderId = order.Id, ProductId = game.Id, Quantity = 1 };

        _gameRepo.Setup(r => r.GetByKeyAsync("key")).ReturnsAsync(game);
        _orderRepo.Setup(r => r.GetOpenOrderByCustomerIdAsync(_stubCustomerId)).ReturnsAsync(order);
        _orderGameRepo.Setup(r => r.GetByOrderAndProductAsync(order.Id, game.Id)).ReturnsAsync(orderGame);

        await _service.AddToCartAsync("key");

        Assert.Equal(2, orderGame.Quantity);
        _orderRepo.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
        _orderGameRepo.Verify(r => r.AddAsync(It.IsAny<OrderGame>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoveFromCartAsyncDeletesOrderIfLastItem()
    {
        var game = new Game { Id = Guid.NewGuid(), Key = "key" };
        var order = new Order { Id = Guid.NewGuid() };
        var orderGame = new OrderGame { OrderId = order.Id, ProductId = game.Id };

        _gameRepo.Setup(r => r.GetByKeyAsync("key")).ReturnsAsync(game);
        _orderRepo.Setup(r => r.GetOpenOrderByCustomerIdAsync(_stubCustomerId)).ReturnsAsync(order);
        _orderGameRepo.Setup(r => r.GetByOrderAndProductAsync(order.Id, game.Id)).ReturnsAsync(orderGame);
        _orderGameRepo.Setup(r => r.GetByOrderIdAsync(order.Id)).ReturnsAsync([orderGame]);

        await _service.RemoveFromCartAsync("key");

        _orderGameRepo.Verify(r => r.Delete(orderGame), Times.Once);
        _orderRepo.Verify(r => r.Delete(order), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task PayAsyncBankReturnsPdfGeneratedResult()
    {
        var order = new Order { Id = Guid.NewGuid(), CustomerId = _stubCustomerId, Status = OrderStatus.Open };
        var orderWithDetails = new Order { Id = order.Id, OrderGames = [new() { Price = 10, Quantity = 1 }] };

        _orderRepo.Setup(r => r.GetOpenOrderByCustomerIdAsync(_stubCustomerId)).ReturnsAsync(order);
        _orderRepo.Setup(r => r.GetByIdWithDetailsAsync(order.Id)).ReturnsAsync(orderWithDetails);

        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

        var request = new PaymentRequestDto { Method = "Bank" };
        var result = await _service.PayAsync(request);

        Assert.NotNull(result.InvoiceFile);
        Assert.Null(result.IBoxResult);
        Assert.Equal(OrderStatus.Paid, order.Status);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task PayAsyncIBoxCallsMicroserviceAndReturnsResult()
    {
        var order = new Order { Id = Guid.NewGuid(), CustomerId = _stubCustomerId, Status = OrderStatus.Open };
        var orderWithDetails = new Order { Id = order.Id, OrderGames = [new() { Price = 10, Quantity = 1 }] };

        _orderRepo.Setup(r => r.GetOpenOrderByCustomerIdAsync(_stubCustomerId)).ReturnsAsync(order);
        _orderRepo.Setup(r => r.GetByIdWithDetailsAsync(order.Id)).ReturnsAsync(orderWithDetails);

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var request = new PaymentRequestDto { Method = "IBox terminal" };
        var result = await _service.PayAsync(request);

        Assert.NotNull(result.IBoxResult);
        Assert.Equal(10, result.IBoxResult.Sum);
        Assert.Equal(OrderStatus.Paid, order.Status);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task PayAsyncVisaCallsMicroserviceAndUpdatesStatus()
    {
        var order = new Order { Id = Guid.NewGuid(), CustomerId = _stubCustomerId, Status = OrderStatus.Open };
        var orderWithDetails = new Order { Id = order.Id, OrderGames = [new() { Price = 10, Quantity = 1 }] };

        _orderRepo.Setup(r => r.GetOpenOrderByCustomerIdAsync(_stubCustomerId)).ReturnsAsync(order);
        _orderRepo.Setup(r => r.GetByIdWithDetailsAsync(order.Id)).ReturnsAsync(orderWithDetails);

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var request = new PaymentRequestDto
        {
            Method = "Visa",
            Model = new VisaPaymentModelDto { CardNumber = "123", Holder = "John Doe" },
        };
        var result = await _service.PayAsync(request);

        Assert.Null(result.InvoiceFile);
        Assert.Null(result.IBoxResult);
        Assert.Equal(OrderStatus.Paid, order.Status);
    }
}
