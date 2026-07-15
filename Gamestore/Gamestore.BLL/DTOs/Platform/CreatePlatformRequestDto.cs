using System.ComponentModel.DataAnnotations;

namespace Gamestore.BLL.DTOs.Platform;

public class CreatePlatformRequestDto
{
    [Required(ErrorMessage = "Platform details are mandatory.")]
    public CreatePlatformDto Platform { get; set; } = null!;
}