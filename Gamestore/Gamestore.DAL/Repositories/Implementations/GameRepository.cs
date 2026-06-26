using Gamestore.DAL.Data;
using Gamestore.DAL.Entities;
using Gamestore.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.DAL.Repositories.Implementations;

public class GameRepository(GamestoreContext context) : Repository<Game>(context), IGameRepository
{
    public async Task<Game?> GetByKeyAsync(string key)
    {
        return await Context.Games
            .Include(game => game.GameGenres)
            .Include(game => game.GamePlatforms)
            .FirstOrDefaultAsync(game => game.Key == key);
    }

    public async Task<IEnumerable<Game>> GetByGenreAsync(Guid genreId)
    {
        return await Context.Games
            .Where(game => game.GameGenres.Any(gamegenre => gamegenre.GenreId == genreId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Game>> GetByPlatformAsync(Guid platformId)
    {
        return await Context.Games
            .Where(game => game.GamePlatforms.Any(gameplatform => gameplatform.PlatformId == platformId))
            .ToListAsync();
    }
}