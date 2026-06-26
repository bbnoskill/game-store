namespace Gamestore.BLL.DTOs.Game;

public class UpdateGameRequestDto
{
    public UpdateGameDto Game { get; set; } = null!;

    public List<Guid> Genres { get; set; } = [];

    public List<Guid> Platforms { get; set; } = [];
}