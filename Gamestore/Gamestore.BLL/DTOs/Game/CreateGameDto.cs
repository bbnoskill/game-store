namespace Gamestore.BLL.DTOs.Game;

public class CreateGameDto
{
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string? Description { get; set; }
}