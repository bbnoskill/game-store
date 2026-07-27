using System.ComponentModel.DataAnnotations;

namespace Gamestore.BLL.DTOs.Comment;

public class CreateCommentDto
{
    [Required(ErrorMessage = "Comment Name is mandatory.")]
    [MinLength(1, ErrorMessage = "Comment Name cannot be empty.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Comment Body is mandatory.")]
    [MinLength(1, ErrorMessage = "Comment Body cannot be empty.")]
    public string Body { get; set; } = string.Empty;
}
