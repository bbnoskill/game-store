using System.ComponentModel.DataAnnotations;

namespace Gamestore.BLL.DTOs.Ban;

public class BanRequestDto
{
    [Required(ErrorMessage = "User name is mandatory.")]
    [MinLength(1, ErrorMessage = "User name cannot be empty.")]
    public string User { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ban duration is mandatory.")]
    public string Duration { get; set; } = string.Empty;
}
