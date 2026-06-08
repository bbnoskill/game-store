using Gamestore.DAL.Entities;

namespace Gamestore.DAL.Repositories.Interfaces;

public interface IPlatformRepository
{
    Task<Platform?> GetByIdAsync(Guid id);

    Task<IEnumerable<Platform>> GetAllAsync();

    Task<IEnumerable<Platform>> GetByGameKeyAsync(string gameKey);

    Task AddAsync(Platform platform);

    Task UpdateAsync(Platform platform);

    Task DeleteAsync(Platform platform);
}