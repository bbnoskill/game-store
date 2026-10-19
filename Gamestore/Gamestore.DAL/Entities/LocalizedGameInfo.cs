namespace Gamestore.DAL.Entities;

/// <summary>Read-only result of dbo.GetGameInfoByLocalization.</summary>
public class LocalizedGameInfo
{
    public Guid Id { get; set; }

    public string Key { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public double Price { get; set; }

    public int Discount { get; set; }

    public int UnitInStock { get; set; }
}
