namespace Gamestore.BLL.DTOs.Game.Filtering;

public class GameFilterDto
{
    public string? Name { get; set; }

    public string? SortBy { get; set; }

    public string? PublishDate { get; set; }

    public string? PageSize { get; set; }

    public int Page { get; set; }

    public double? MinPrice { get; set; }

    public double? MaxPrice { get; set; }

    public List<Guid?> Publishers { get; set; }

    public List<Guid?> Platforms { get; set; }

    public List<Guid?> Genres { get; set; }
}