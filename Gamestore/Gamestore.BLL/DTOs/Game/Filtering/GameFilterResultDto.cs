namespace Gamestore.BLL.DTOs.Game.Filtering;

public class GameFilterResultDto
{
    public IEnumerable<GameDto> Games { get; set; }

    public int TotalPages { get; set; }

    public int CurrentPage { get; set; }
}