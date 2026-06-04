namespace Gamestore.BLL.DTOs.Game;

public class CreateGameRequestDto
{
    public CreateGameDto Game { get; set; } = null!;

    public List<Guid> Genres { get; set; } = [];

    public List<Guid> Platforms { get; set; } = [];
}