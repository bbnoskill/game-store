namespace Gamestore.DAL.Entities;

public class Publisher
{
    public Guid Id { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public string? HomePage { get; set; }

    public string? Description { get; set; }

    public string? ContactName { get; set; }

    public string? ContactTitle { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Region { get; set; }

    public string? PostalCode { get; set; }

    public string? Country { get; set; }

    public string? Phone { get; set; }

    public string? Fax { get; set; }

    public ICollection<Game> Games { get; set; } = [];

    public ICollection<PublisherLocalization> Localizations { get; set; } = [];
}