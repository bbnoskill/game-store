using Gamestore.BLL.DTOs.Game.Filtering;
using Gamestore.BLL.Filtering.Interfaces;
using Gamestore.DAL.Entities;

namespace Gamestore.BLL.Filtering.Implementations;

public class PublishDateFilterStep : IGameFilterStep
{
    public IQueryable<Game> Execute(IQueryable<Game> query, GameFilterDto filter)
    {
        if (string.IsNullOrEmpty(filter.PublishDate))
        {
            return query;
        }

        var cutoff = filter.PublishDate switch
        {
            "last week" => DateTime.UtcNow.AddDays(-7),
            "last month" => DateTime.UtcNow.AddMonths(-1),
            "last year" => DateTime.UtcNow.AddYears(-1),
            "2 years" => DateTime.UtcNow.AddYears(-2),
            "3 years" => DateTime.UtcNow.AddYears(-3),
            _ => (DateTime?)null,
        };
        return cutoff.HasValue
            ? query.Where(g => g.ReleaseDate >= cutoff.Value)
            : query;
    }
}