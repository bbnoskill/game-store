using System.ComponentModel.DataAnnotations.Schema;

namespace Gamestore.DAL.Entities;

public class Order
{
    public Guid Id { get; set; }

    public DateTime? Date { get; set; }

    public Guid CustomerId { get; set; }

    public OrderStatus Status { get; set; }

    public DateTime? ShippedDate { get; set; }

    public int? ShipVia { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Freight { get; set; }

    public string? ShipName { get; set; }

    public string? ShipAddress { get; set; }

    public string? ShipCity { get; set; }

    public string? ShipRegion { get; set; }

    public string? ShipPostalCode { get; set; }

    public string? ShipCountry { get; set; }

    public ICollection<OrderGame> OrderGames { get; set; } = [];
}