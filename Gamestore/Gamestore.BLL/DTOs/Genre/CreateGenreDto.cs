using System.ComponentModel.DataAnnotations;

namespace Gamestore.BLL.DTOs.Genre;

public class CreateGenreDto
{
    [Required(ErrorMessage = "Genre Name is mandatory.")]
    [MinLength(1, ErrorMessage = "Genre Name cannot be empty.")]
    public string Name { get; set; } = string.Empty;

    public Guid? ParentGenreId { get; set; }
}