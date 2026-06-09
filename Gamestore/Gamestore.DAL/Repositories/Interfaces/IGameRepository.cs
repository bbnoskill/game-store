using Gamestore.DAL.Entities;

namespace Gamestore.DAL.Repositories.Interfaces;

public interface IGameRepository
{
    Task<Game?> GetByKeyAsync(string key);

    Task<Game?> GetByIdAsync(Guid id);

    Task<IEnumerable<Game>> GetAllAsync();

    Task<IEnumerable<Game>> GetByGenreAsync(Guid genreId);

    Task<IEnumerable<Game>> GetByPlatformAsync(Guid platformId);

    Task AddAsync(Game game);

    Task UpdateAsync(Game game);

    Task DeleteAsync(Game game);

    Task<int> GetCountAsync();
}