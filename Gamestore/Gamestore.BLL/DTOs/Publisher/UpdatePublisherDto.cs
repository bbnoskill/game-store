using System.ComponentModel.DataAnnotations;

namespace Gamestore.BLL.DTOs.Publisher;

public class UpdatePublisherDto
{
    [Required(ErrorMessage = "Id is required.")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Company name is mandatory.")]
    [MinLength(1, ErrorMessage = "Company name cannot be empty.")]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Homepage is mandatory.")]
    public string HomePage { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is mandatory.")]
    public string Description { get; set; } = string.Empty;
}