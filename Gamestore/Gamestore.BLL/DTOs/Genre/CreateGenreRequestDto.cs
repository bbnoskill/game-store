using System.ComponentModel.DataAnnotations;

namespace Gamestore.BLL.DTOs.Genre;

public class CreateGenreRequestDto
{
    [Required(ErrorMessage = "Genre details are mandatory.")]
    public CreateGenreDto Genre { get; set; } = null!;
}