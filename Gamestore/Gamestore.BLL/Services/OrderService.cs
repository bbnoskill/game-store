using System.Net.Http.Json;
using Gamestore.BLL.DTOs.Order;
using Gamestore.BLL.Interfaces;
using Gamestore.DAL.Entities;
using Gamestore.DAL.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Gamestore.BLL.Services;

public class OrderService(IUnitOfWork unitOfWork, ILogger<OrderService> logger,
    IHttpClientFactory clientFactory, IConfiguration configuration) : IOrderService
{
    private const int MaxRetries = 5;
    private static readonly Guid StubCustomerId = Guid.Parse("5aa1c97e-e6b3-497c-8e00-270e96aa0b63");
    private static readonly List<PaymentMethodDto> PaymentMethods =
    [
        new() { Title = "Bank", Description = "Pay via bank invoice", ImageUrl = "https://img.icons8.com/ios/50/bank-building.png" },
        new() { Title = "IBox terminal", Description = "Pay via IBox terminal", ImageUrl = "https://img.icons8.com/ios/50/atm.png" },
        new() { Title = "Visa", Description = "Pay with Visa card", ImageUrl = "https://img.icons8.com/ios/50/visa.png" },
    ];

    public async Task AddToCartAsync(string gameKey)
    {
        logger.LogInformation("Fetching game by key: {Key}", gameKey);
        var game = await unitOfWork.Games.GetByKeyAsync(gameKey)
                   ?? throw new KeyNotFoundException($"Game with key '{gameKey}' was not found.");
        if (game.UnitInStock <= 0)
        {
            throw new ArgumentException("No games in stock.");
        }

        var existingOrder = await unitOfWork.Orders.GetOpenOrderByCustomerIdAsync(StubCustomerId);
        bool isNewOrder = existingOrder is null;
        var order = existingOrder ?? new Order
        {
            CustomerId = StubCustomerId,
            Date = DateTime.Now,
            Id = Guid.NewGuid(),
            Status = OrderStatus.Open,
        };

        if (isNewOrder)
        {
            await unitOfWork.Orders.AddAsync(order);
        }

        var existingOrderGame = await unitOfWork.OrderGames.GetByOrderAndProductAsync(order.Id, game.Id);
        bool isNewOrderGame = existingOrderGame is null;
        var orderGame = existingOrderGame ?? new OrderGame
        {
            OrderId = order.Id,
            ProductId = game.Id,
            Discount = game.Discount,
            Price = game.Price,
            Quantity = 0,
        };
        orderGame.Quantity++;
        if (orderGame.Quantity > game.UnitInStock)
        {
            throw new ArgumentException("The order doesn't have enough games in stock.");
        }

        if (isNewOrderGame)
        {
            await unitOfWork.OrderGames.AddAsync(orderGame);
        }

        await unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveFromCartAsync(string gameKey)
    {
        var game = await unitOfWork.Games.GetByKeyAsync(gameKey)
                    ?? throw new KeyNotFoundException($"Game with key '{gameKey}' was not found.");
        var order = await unitOfWork.Orders.GetOpenOrderByCustomerIdAsync(StubCustomerId)
                    ?? throw new KeyNotFoundException($"Order with key '{gameKey}' was not found.");
        var orderGame = await unitOfWork.OrderGames.GetByOrderAndProductAsync(order.Id, game.Id)
                    ?? throw new KeyNotFoundException($"Game is not in the cart");
        unitOfWork.OrderGames.Delete(orderGame);

        var remainingItems = await unitOfWork.OrderGames.GetByOrderIdAsync(order.Id);
        if (!remainingItems.Any(og => og.ProductId != game.Id))
        {
            unitOfWork.Orders.Delete(order);
        }

        await unitOfWork.SaveChangesAsync();
    }

    public async Task<PaymentResultDto> PayAsync(PaymentRequestDto request)
    {
        var order = await unitOfWork.Orders.GetOpenOrderByCustomerIdAsync(StubCustomerId)
                  ?? throw new KeyNotFoundException("Order was not found.");
        var orderWithDetails = await unitOfWork.Orders.GetByIdWithDetailsAsync(order.Id)
            ?? throw new KeyNotFoundException($"Order details for id '{order.Id}' were not found.");
        order.Status = OrderStatus.Checkout;
        await unitOfWork.SaveChangesAsync();
        var sum = orderWithDetails.OrderGames.Sum(og => og.Price * og.Quantity * (1 - (og.Discount / 100.0)));
        var result = new PaymentResultDto();
        try
        {
            switch (request.Method)
            {
                case "Bank":
                    result.InvoiceFile = GenerateBankInvoice(order, sum);
                    break;
                case "IBox terminal":
                    result.IBoxResult = await ProcessMicroservicePaymentAsync(order, sum, request);
                    break;
                case "Visa":
                    await ProcessMicroservicePaymentAsync(order, sum, request);
                    break;

                default:
                    throw new Exception($"Unsupported method: {request.Method}");
            }

            order.Status = OrderStatus.Paid;
            order.Date = DateTime.UtcNow;
        }
        catch (Exception ex) when (ex is not ArgumentException and not KeyNotFoundException)
        {
            logger.LogError(ex, "Payment failed for order {OrderId}", order.Id);
            order.Status = OrderStatus.Cancelled;
        }

        await unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersAsync()
    {
        logger.LogInformation("Fetching all orders.");
        var orders = (await unitOfWork.Orders.GetAllAsync()).ToList();
        logger.LogInformation("Fetched {Count} orders.", orders.Count);
        return orders.Select(order => new OrderDto()
        {
            CustomerId = order.CustomerId,
            Id = order.Id,
            Date = order.Date,
        });
    }

    public async Task<OrderDto?> GetOrderByIdAsync(Guid id)
    {
        logger.LogInformation("Fetching order by id: {Id}", id);
        var order = await unitOfWork.Orders.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Order with id '{id}' was not found.");
        return new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Date = order.Date,
        };
    }

    public async Task<IEnumerable<OrderDetailsDto>> GetOrderDetailsAsync(Guid orderId)
    {
        var order = await unitOfWork.Orders.GetByIdWithDetailsAsync(orderId)
            ?? throw new KeyNotFoundException($"Order with id '{orderId}' was not found.");
        return order.OrderGames.Select(og => new OrderDetailsDto
        {
            ProductId = og.ProductId,
            Price = og.Price,
            Discount = og.Discount,
            Quantity = og.Quantity,
        });
    }

    public async Task<IEnumerable<OrderDetailsDto>> GetCartAsync()
    {
        var order = await unitOfWork.Orders.GetOpenOrderByCustomerIdAsync(StubCustomerId)
            ?? throw new KeyNotFoundException($"No orders for id {StubCustomerId} were found.");
        var orderGames = await unitOfWork.OrderGames.GetByOrderIdAsync(order.Id);
        return orderGames.Select(og => new OrderDetailsDto
        {
            ProductId = og.ProductId,
            Price = og.Price,
            Discount = og.Discount,
            Quantity = og.Quantity,
        });
    }

    public IEnumerable<PaymentMethodDto> GetPaymentMethods()
    {
        return PaymentMethods;
    }

    private byte[] GenerateBankInvoice(Order order, double sum)
    {
        var validityDays = configuration.GetValue<int>("Payment:BankInvoiceValidityDays");

        // Тут будe генерувати PDF
        // Поки що заглушка — потім заміню на реальну
        var content = $"""
                       Invoice
                       User ID: {order.CustomerId}
                       Order ID: {order.Id}
                       Creation Date: {DateTime.UtcNow}
                       Valid Until: {DateTime.UtcNow.AddDays(validityDays)}
                       Sum: {sum:F2}
                       """;

        return System.Text.Encoding.UTF8.GetBytes(content);
    }

    private async Task<IBoxPaymentResultDto> ProcessMicroservicePaymentAsync(
        Order order, double sum, PaymentRequestDto request)
    {
        var microserviceUrl = configuration["Payment:MicroserviceUrl"];
        var client = clientFactory.CreateClient();

        for (int attempt = 0; attempt < MaxRetries; attempt++)
        {
            var payload = request.Method == "Visa"
                ? (object)new { request.Method, request.Model }
                : new { request.Method };

            var response = await client.PostAsJsonAsync($"{microserviceUrl}/api/payments", payload);

            if (response.IsSuccessStatusCode)
            {
                return new IBoxPaymentResultDto
                {
                    UserId = order.CustomerId,
                    OrderId = order.Id,
                    PaymentDate = DateTime.UtcNow,
                    Sum = sum,
                };
            }

            logger.LogWarning("Payment attempt {Attempt} failed for order {OrderId}", attempt + 1, order.Id);
        }

        throw new InvalidOperationException("Payment failed after all retry attempts.");
    }
}