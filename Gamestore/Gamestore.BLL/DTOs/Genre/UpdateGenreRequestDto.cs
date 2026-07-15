using System.ComponentModel.DataAnnotations;

namespace Gamestore.BLL.DTOs.Genre;

public class UpdateGenreRequestDto
{
    [Required(ErrorMessage = "Genre details are mandatory.")]
    public UpdateGenreDto Genre { get; set; } = null!;
}