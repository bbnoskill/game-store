using System.Globalization;
using Gamestore.BLL.Interfaces;

namespace Gamestore.BLL.Services;

/// <summary>Resolves supported languages by quality and header order.</summary>
public class LanguageService : ILanguageService
{
    public string NormalizeLanguageCode(string? acceptLanguageHeader)
    {
        var candidates = (acceptLanguageHeader ?? string.Empty)
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(ParseLanguage)
            .Where(candidate => candidate.Quality > 0)
            .OrderByDescending(candidate => candidate.Quality);

        foreach (var (language, _) in candidates)
        {
            if (language is "en" or "uk" or "ja")
            {
                return language;
            }

            if (language == "*")
            {
                return "en";
            }
        }

        return "en";
    }

    private static (string Language, double Quality) ParseLanguage(string value)
    {
        var parts = value.Split(';', StringSplitOptions.TrimEntries);
        var language = parts[0].Split('-')[0].ToLowerInvariant();
        if (language == "ua")
        {
            language = "uk";
        }

        double quality = 1;
        foreach (var parameter in parts.Skip(1))
        {
            if (!parameter.StartsWith("q=", StringComparison.OrdinalIgnoreCase)
                || !double.TryParse(parameter.AsSpan(2), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out quality)
                || quality is < 0 or > 1)
            {
                return (language, 0);
            }
        }

        return (language, quality);
    }
}
