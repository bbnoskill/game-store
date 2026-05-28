namespace Gamestore.BLL.DTOs.Game;

public class UpdateGameRequestDto
{
    public UpdateGameRequestDto Game { get; set; } = null!;
    public List<Guid> Genres { get; set; } = new();
    public List<Guid> Platforms { get; set; } = new();
}