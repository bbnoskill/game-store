namespace Gamestore.BLL.DTOs.Genre;

public class CreateGenreDto
{
    public string Name { get; set; } = string.Empty;

    public Guid? ParentGenreId { get; set; }
}