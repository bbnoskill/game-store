using System.ComponentModel.DataAnnotations;

namespace Gamestore.BLL.DTOs.Order;

public class VisaPaymentModelDto
{
    public string Holder { get; init; } = string.Empty;

    [CreditCard]
    public string CardNumber { get; init; } = string.Empty;

    [Range(1, 12)]
    public int MonthExpire { get; init; }

    public int YearExpire { get; init; }

    public int Cvv2 { get; init; }
}