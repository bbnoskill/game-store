using Gamestore.DAL.Entities;

namespace Gamestore.DAL.Repositories.Interfaces;

public interface IPlatformRepository : IRepository<Platform>
{
    Task<IEnumerable<Platform>> GetByGameKeyAsync(string gameKey);
}