namespace Gamestore.DAL.Entities;

public class PublisherLocalization
{
    public Guid Id { get; set; }

    public Guid PublisherId { get; set; }

    public Publisher? Publisher { get; set; }

    public string LanguageCode { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public string? Description { get; set; }
}
