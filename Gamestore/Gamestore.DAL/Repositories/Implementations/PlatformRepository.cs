using Gamestore.DAL.Data;
using Gamestore.DAL.Entities;
using Gamestore.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.DAL.Repositories.Implementations;

public class PlatformRepository(GamestoreContext context) : Repository<Platform>(context), IPlatformRepository
{
    public async Task<IEnumerable<Platform>> GetByGameKeyAsync(string gameKey)
    {
        return await Context.Platforms
            .Where(platform => platform.GamePlatforms.Any(gamePlatform => gamePlatform.Game.Key == gameKey))
            .ToListAsync();
    }
}