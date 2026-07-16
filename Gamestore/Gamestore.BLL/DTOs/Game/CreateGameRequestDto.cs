using System.ComponentModel.DataAnnotations;

namespace Gamestore.BLL.DTOs.Game;

public class CreateGameRequestDto
{
    [Required(ErrorMessage = "Game details are mandatory.")]
    public CreateGameDto Game { get; set; } = null!;

    [Required(ErrorMessage = "At least one genre should be provided.")]
    [MinLength(1, ErrorMessage = "Game must have at least one genre.")]
    public List<Guid> Genres { get; set; } = [];

    [Required(ErrorMessage = "At least one platform should be provided.")]
    [MinLength(1, ErrorMessage = "Game must have at least one platform.")]
    public List<Guid> Platforms { get; set; } = [];

    [Required(ErrorMessage = "Publisher is mandatory.")]
    public Guid Publisher { get; set; }
}