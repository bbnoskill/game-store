namespace Gamestore.DAL.Entities;

public class PlatformLocalization
{
    public Guid Id { get; set; }

    public Guid PlatformId { get; set; }

    public Platform? Platform { get; set; }

    public string LanguageCode { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;
}
