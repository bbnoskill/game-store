using Gamestore.DAL.Entities;

namespace Gamestore.BLL.Services;

internal static class EntityLocalization
{
    public static void Update(Game entity, string languageCode, string name, string? description)
    {
        var language = new LanguageService().NormalizeLanguageCode(languageCode);
        var translation = entity.Localizations.FirstOrDefault(l => l.LanguageCode == language);
        if (translation == null)
        {
            translation = new GameLocalization
            {
                GameId = entity.Id,
                LanguageCode = language,
            };
            entity.Localizations.Add(translation);
        }

        translation.Name = name;
        translation.Description = description;

        if (language == "en")
        {
            entity.Name = name;
            entity.Description = description;
        }
    }

    public static void Update(Genre entity, string languageCode, string name)
    {
        var language = new LanguageService().NormalizeLanguageCode(languageCode);
        var translation = entity.Localizations.FirstOrDefault(l => l.LanguageCode == language);
        if (translation == null)
        {
            translation = new GenreLocalization
            {
                GenreId = entity.Id,
                LanguageCode = language,
            };
            entity.Localizations.Add(translation);
        }

        translation.Name = name;

        if (language == "en")
        {
            entity.Name = name;
        }
    }

    public static void Update(Platform entity, string languageCode, string type)
    {
        var language = new LanguageService().NormalizeLanguageCode(languageCode);
        var translation = entity.Localizations.FirstOrDefault(l => l.LanguageCode == language);
        if (translation == null)
        {
            translation = new PlatformLocalization
            {
                PlatformId = entity.Id,
                LanguageCode = language,
            };
            entity.Localizations.Add(translation);
        }

        translation.Type = type;

        if (language == "en")
        {
            entity.Type = type;
        }
    }

    public static void Update(Publisher entity, string languageCode, string companyName, string? description)
    {
        var language = new LanguageService().NormalizeLanguageCode(languageCode);
        var translation = entity.Localizations.FirstOrDefault(l => l.LanguageCode == language);
        if (translation == null)
        {
            translation = new PublisherLocalization
            {
                PublisherId = entity.Id,
                LanguageCode = language,
            };
            entity.Localizations.Add(translation);
        }

        translation.CompanyName = companyName;
        translation.Description = description;

        if (language == "en")
        {
            entity.CompanyName = companyName;
            entity.Description = description;
        }
    }
}
