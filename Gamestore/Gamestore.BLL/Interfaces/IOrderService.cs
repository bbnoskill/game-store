using Gamestore.BLL.DTOs.Order;

namespace Gamestore.BLL.Interfaces;

public interface IOrderService
{
    Task AddToCartAsync(string gameKey);

    Task RemoveFromCartAsync(string gameKey);

    Task<IEnumerable<OrderDto>> GetOrdersAsync();

    Task<OrderDto?> GetOrderByIdAsync(Guid id);

    Task<IEnumerable<OrderDetailsDto>> GetOrderDetailsAsync(Guid orderId);

    Task<IEnumerable<OrderDetailsDto>> GetCartAsync();

    IEnumerable<PaymentMethodDto> GetPaymentMethods();

    Task<PaymentResultDto> PayAsync(PaymentRequestDto request);
}