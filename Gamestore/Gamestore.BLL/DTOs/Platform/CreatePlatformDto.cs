using System.ComponentModel.DataAnnotations;

namespace Gamestore.BLL.DTOs.Platform;

public class CreatePlatformDto
{
    [Required(ErrorMessage = "Type is mandatory.")]
    [MinLength(1, ErrorMessage = "Type cannot be empty.")]
    public string Type { get; set; } = string.Empty;
}