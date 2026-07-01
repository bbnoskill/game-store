namespace Gamestore.DAL.Entities;

public class Game
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Key { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<GameGenre> GameGenres { get; set; } = [];

    public ICollection<GamePlatform> GamePlatforms { get; set; } = [];
}