using Gamestore.BLL.DTOs.Game.Filtering;
using Gamestore.BLL.Filtering.Interfaces;
using Gamestore.DAL.Entities;

namespace Gamestore.BLL.Filtering.Implementations;

public class SortStep : IGameFilterStep
{
    public IQueryable<Game> Execute(IQueryable<Game> query, GameFilterDto filter)
    {
        return filter.SortBy switch
        {
            "Most popular" => query.OrderByDescending(g => g.ViewCount),
            "Most commented" => query.OrderByDescending(g => g.GameComments.Count),
            "Price ASC" => query.OrderBy(g => g.Price),
            "Price DESC" => query.OrderByDescending(g => g.Price),
            "New" => query.OrderByDescending(g => g.ReleaseDate),
            _ => query,
        };
    }
}