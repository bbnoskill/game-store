namespace Gamestore.DAL.Entities;

public class GenreLocalization
{
    public Guid Id { get; set; }

    public Guid GenreId { get; set; }

    public Genre? Genre { get; set; }

    public string LanguageCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}
