using System.Net.Http.Json;
using Gamestore.BLL.DTOs.Order;
using Gamestore.BLL.Interfaces;
using Gamestore.DAL.Entities;
using Gamestore.DAL.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
                    throw new ArgumentException($"Unsupported method: {request.Method}");
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
        var creationDate = DateTime.UtcNow;
        var validUntil = creationDate.AddDays(validityDays);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);

                page.Header().Text("Game Store Invoice")
                    .SemiBold().FontSize(24).FontColor(Colors.Blue.Medium);

                page.Content().PaddingVertical(10).Column(column =>
                {
                    column.Spacing(8);

                    column.Item().Text($"User ID: {order.CustomerId}");
                    column.Item().Text($"Order ID: {order.Id}");
                    column.Item().Text($"Creation Date: {creationDate:yyyy-MM-dd HH:mm}");
                    column.Item().Text($"Valid Until: {validUntil:yyyy-MM-dd HH:mm}");

                    column.Item().PaddingTop(10).Text($"Total Sum: ${sum:F2}")
                        .SemiBold().FontSize(16);
                });

                page.Footer().AlignCenter()
                    .Text("Thank you for your purchase!");
            });
        });

        return document.GeneratePdf();
    }

    private async Task<IBoxPaymentResultDto> ProcessMicroservicePaymentAsync(
        Order order, double sum, PaymentRequestDto request)
    {
        var microserviceUrl = configuration["Payment:MicroserviceUrl"];
        var client = clientFactory.CreateClient();

        for (int attempt = 0; attempt < MaxRetries; attempt++)
        {
            HttpResponseMessage response;

            if (request.Method == "Visa")
            {
                var visaPayload = new
                {
                    transactionAmount = sum,
                    cardHolderName = request.Model!.Holder,
                    cardNumber = request.Model.CardNumber,
                    expirationMonth = request.Model.MonthExpire,
                    expirationYear = request.Model.YearExpire,
                    cvv = request.Model.Cvv2,
                };
                response = await client.PostAsJsonAsync($"{microserviceUrl}/api/payments/visa", visaPayload);
            }
            else
            {
                var iboxPayload = new
                {
                    transactionAmount = sum,
                    accountNumber = order.CustomerId,
                    invoiceNumber = order.Id,
                };
                response = await client.PostAsJsonAsync($"{microserviceUrl}/api/payments/ibox", iboxPayload);
            }

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