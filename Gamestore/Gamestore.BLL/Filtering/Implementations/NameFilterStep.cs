using Gamestore.BLL.DTOs.Game.Filtering;
using Gamestore.BLL.Filtering.Interfaces;
using Gamestore.DAL.Entities;

namespace Gamestore.BLL.Filtering.Implementations;

public class NameFilterStep : IGameFilterStep
{
    public IQueryable<Game> Execute(IQueryable<Game> query, GameFilterDto filter)
    {
        return string.IsNullOrEmpty(filter.Name) || filter.Name.Length < 3
            ? query
            : query.Where(g => (g.Localizations.Where(l => l.LanguageCode == filter.LanguageCode).Select(l => l.Name).FirstOrDefault()
                ?? g.Localizations.Where(l => l.LanguageCode == "en").Select(l => l.Name).FirstOrDefault()
                ?? g.Name).Contains(filter.Name));
    }
}