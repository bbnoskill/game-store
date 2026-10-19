namespace Gamestore.BLL.DTOs.Order;

public class OrderDetailsDto
{
    public Guid ProductId { get; set; }

    public string? ProductName { get; set; }

    public string? GameKey { get; set; }

    public double Price { get; set; }

    public int Quantity { get; set; }

    public int Discount { get; set; }
}