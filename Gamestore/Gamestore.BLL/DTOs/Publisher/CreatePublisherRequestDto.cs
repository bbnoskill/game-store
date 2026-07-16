using System.ComponentModel.DataAnnotations;

namespace Gamestore.BLL.DTOs.Publisher;

public class CreatePublisherRequestDto
{
    [Required(ErrorMessage = "Publisher details are mandatory.")]
    public CreatePublisherDto Publisher { get; set; } = null!;
}