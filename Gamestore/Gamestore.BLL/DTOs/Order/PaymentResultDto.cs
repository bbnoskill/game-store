namespace Gamestore.BLL.DTOs.Order;

public class PaymentResultDto
{
    public byte[]? InvoiceFile { get; set; }

    public IBoxPaymentResultDto? IBoxResult { get; set; }
}