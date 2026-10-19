namespace Gamestore.BLL.DTOs.Game.Filtering;

public class GameFilterDto
{
    public string LanguageCode { get; set; } = "en";

    public string? Name { get; set; }

    public string? SortBy { get; set; }

    public string? Sort { get => SortBy; set => SortBy = value; }

    public string? PublishDate { get; set; }

    public string? DatePublishing { get => PublishDate; set => PublishDate = value; }

    public string? PageSize { get; set; }

    public string? PageCount { get => PageSize; set => PageSize = value; }

    public int Page { get; set; }

    public double? MinPrice { get; set; }

    public double? MaxPrice { get; set; }

    public List<Guid?>? Publishers { get; set; }

    public List<Guid?>? Platforms { get; set; }

    public List<Guid?>? Genres { get; set; }
}