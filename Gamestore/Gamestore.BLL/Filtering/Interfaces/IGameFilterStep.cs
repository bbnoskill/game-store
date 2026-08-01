using Gamestore.BLL.DTOs.Game.Filtering;
using Gamestore.DAL.Entities;

namespace Gamestore.BLL.Filtering.Interfaces;

public interface IGameFilterStep
{
    IQueryable<Game> Execute(IQueryable<Game> query, GameFilterDto filter);
}