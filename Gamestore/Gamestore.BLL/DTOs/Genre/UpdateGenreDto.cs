namespace Gamestore.BLL.DTOs.Genre;

public class UpdateGenreDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public Guid? ParentGenreId { get; set; }
}