using Gamestore.DAL.Entities;

namespace Gamestore.DAL.Repositories.Interfaces;

public interface IGenreRepository : IRepository<Genre>
{
    Task<IEnumerable<Genre>> GetByGameKeyAsync(string gameKey);

    Task<IEnumerable<Genre>> GetByParentIdAsync(Guid parentId);
}