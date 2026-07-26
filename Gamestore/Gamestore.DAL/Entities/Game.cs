namespace Gamestore.DAL.Entities;

public class Game
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Key { get; set; } = string.Empty;

    public string? Description { get; set; }

    public double Price { get; set; }

    public int UnitInStock { get; set; }

    public int Discount { get; set; }

    public Guid PublisherId { get; set; }

    public Publisher? Publisher { get; set; }

    public ICollection<GameGenre> GameGenres { get; set; } = [];

    public ICollection<GamePlatform> GamePlatforms { get; set; } = [];

    public ICollection<OrderGame> OrderGames { get; set; } = [];
}