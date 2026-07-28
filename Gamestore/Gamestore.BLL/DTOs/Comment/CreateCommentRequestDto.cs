using System.ComponentModel.DataAnnotations;

namespace Gamestore.BLL.DTOs.Comment;

public class CreateCommentRequestDto
{
    [Required(ErrorMessage = "Comment details are mandatory.")]
    public CreateCommentDto Comment { get; set; } = null!;

    public string? ParentId { get; set; }

    public string? Action { get; set; }
}