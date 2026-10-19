using Gamestore.BLL.DTOs.Order;

namespace Gamestore.BLL.Interfaces;

public interface IOrderService
{
    Task AddToCartAsync(string gameKey);

    Task RemoveFromCartAsync(string gameKey);

    Task<IEnumerable<OrderDto>> GetOrdersAsync();

    Task<IEnumerable<OrderDto>> GetHistoryAsync(DateTime? startDate, DateTime? endDate);

    Task<OrderDto?> GetOrderByIdAsync(Guid id);

    Task<IEnumerable<OrderDetailsDto>> GetOrderDetailsAsync(Guid orderId, string languageCode = "en");

    Task<IEnumerable<OrderDetailsDto>> GetCartAsync(string languageCode = "en");

    IEnumerable<PaymentMethodDto> GetPaymentMethods();

    Task<PaymentResultDto> PayAsync(PaymentRequestDto request);
}