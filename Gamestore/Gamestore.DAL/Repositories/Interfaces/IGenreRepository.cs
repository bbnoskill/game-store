using Gamestore.DAL.Entities;

namespace Gamestore.DAL.Repositories.Interfaces;

public interface IGenreRepository
{
    Task<Genre?> GetByIdAsync(Guid id);

    Task<IEnumerable<Genre>> GetAllAsync();

    Task<IEnumerable<Genre>> GetByGameKeyAsync(string gameKey);

    Task<IEnumerable<Genre>> GetByParentIdAsync(Guid parentId);

    Task AddAsync(Genre genre);

    Task UpdateAsync(Genre genre);

    Task DeleteAsync(Genre genre);
}