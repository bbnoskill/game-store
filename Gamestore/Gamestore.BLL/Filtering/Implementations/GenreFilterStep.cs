using Gamestore.BLL.DTOs.Game.Filtering;
using Gamestore.BLL.Filtering.Interfaces;
using Gamestore.DAL.Entities;

namespace Gamestore.BLL.Filtering.Implementations;

public class GenreFilterStep : IGameFilterStep
{
    public IQueryable<Game> Execute(IQueryable<Game> query, GameFilterDto filter)
    {
        return filter.Genres is not { Count: > 0 }
            ? query
            : query.Where(g => g.GameGenres.Any(gg => filter.Genres.Contains(gg.GenreId)));
    }
}