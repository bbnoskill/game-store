using System.ComponentModel.DataAnnotations;

namespace Gamestore.BLL.DTOs.Publisher;

public class UpdatePublisherRequestDto
{
    [Required(ErrorMessage = "Publisher details are mandatory.")]
    public UpdatePublisherDto Publisher { get; set; } = null!;
}