using Gamestore.BLL.DTOs.Game.Filtering;
using Gamestore.BLL.Filtering.Interfaces;
using Gamestore.DAL.Entities;

namespace Gamestore.BLL.Filtering;

public class GameFilterPipeline(IEnumerable<IGameFilterStep> steps)
{
    public IQueryable<Game> Execute(IQueryable<Game> query, GameFilterDto filter)
    {
        foreach (var step in steps)
        {
            query = step.Execute(query, filter);
        }

        return query;
    }
}