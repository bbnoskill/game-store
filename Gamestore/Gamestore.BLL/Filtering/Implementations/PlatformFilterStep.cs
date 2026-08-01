using Gamestore.BLL.DTOs.Game.Filtering;
using Gamestore.BLL.Filtering.Interfaces;
using Gamestore.DAL.Entities;

namespace Gamestore.BLL.Filtering.Implementations;

public class PlatformFilterStep : IGameFilterStep
{
    public IQueryable<Game> Execute(IQueryable<Game> query, GameFilterDto filter)
    {
        return filter.Platforms is not { Count: > 0 }
            ? query
            : query.Where(g => g.GamePlatforms.Any(gp => filter.Platforms.Contains(gp.PlatformId)));
    }
}