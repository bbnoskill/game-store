using System.ComponentModel.DataAnnotations;

namespace Gamestore.BLL.DTOs.Order;

public class PaymentRequestDto
{
    [Required]
    public string Method { get; set; } = string.Empty;

    public VisaPaymentModelDto? Model { get; init; }
}