using System.ComponentModel.DataAnnotations;

namespace Gamestore.BLL.DTOs.Platform;

public class UpdatePlatformRequestDto
{
    [Required(ErrorMessage = "Platform details are mandatory.")]
    public UpdatePlatformDto Platform { get; set; } = null!;
}