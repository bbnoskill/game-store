namespace Gamestore.BLL.DTOs.Comment;

public class CommentDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Body { get; set; }

    public List<CommentDto> ChildComments { get; set; } = [];
}