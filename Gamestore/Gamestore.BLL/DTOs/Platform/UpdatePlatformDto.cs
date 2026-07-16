using System.ComponentModel.DataAnnotations;

namespace Gamestore.BLL.DTOs.Platform;

public class UpdatePlatformDto
{
    [Required(ErrorMessage = "Id is required.")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Type is mandatory.")]
    [MinLength(1, ErrorMessage = "Type cannot be empty.")]
    public string Type { get; set; } = string.Empty;
}