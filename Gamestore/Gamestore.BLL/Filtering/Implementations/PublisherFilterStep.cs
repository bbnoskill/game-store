using Gamestore.BLL.DTOs.Game.Filtering;
using Gamestore.BLL.Filtering.Interfaces;
using Gamestore.DAL.Entities;

namespace Gamestore.BLL.Filtering.Implementations;

public class PublisherFilterStep : IGameFilterStep
{
    public IQueryable<Game> Execute(IQueryable<Game> query, GameFilterDto filter)
    {
        return filter.Publishers is not { Count: > 0 }
            ? query
            : query.Where(g => filter.Publishers.Contains(g.PublisherId));
    }
}