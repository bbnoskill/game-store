namespace Gamestore.BLL.DTOs.Order;

public class IboxPaymentResultDto
{
    public Guid UserId { get; set; }

    public Guid OrderId { get; set; }

    public DateTime PaymentDate { get; set; }

    public double Sum { get; set; }
}