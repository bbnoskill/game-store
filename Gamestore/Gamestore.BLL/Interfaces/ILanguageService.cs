namespace Gamestore.BLL.Interfaces;

/// <summary>
/// Service for resolving and normalizing language codes.
/// </summary>
public interface ILanguageService
{
    /// <summary>
    /// Normalizes a raw Accept-Language header value to one of the supported language codes.
    /// Returns "en" if the value is null, empty, or unsupported.
    /// </summary>
    string NormalizeLanguageCode(string? acceptLanguageHeader);
}
