namespace Gamestore.BLL.DTOs.Order;

public class PaymentResultDto
{
    public byte[]? InvoiceFile { get; set; }

    public IboxPaymentResultDto? IBoxResult { get; set; }
}