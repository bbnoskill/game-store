namespace Gamestore.DAL.Entities;
public class Genre
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? ParentGenreId { get; set; }
    
    public Genre? ParentGenre { get; set; }
    public ICollection<Genre> SubGenres { get; set; } = new List<Genre>();
    public ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();
}