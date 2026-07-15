using Gamestore.DAL.Entities;

namespace Gamestore.DAL.Repositories.Interfaces;

public interface IGameRepository : IRepository<Game>
{
    Task<Game?> GetByKeyAsync(string key);

    Task<IEnumerable<Game>> GetByGenreAsync(Guid genreId);

    Task<IEnumerable<Game>> GetByPlatformAsync(Guid platformId);

    Task<IEnumerable<Game>> GetByPublisherAsync(string companyName);

    Task<int> GetCountAsync();
}