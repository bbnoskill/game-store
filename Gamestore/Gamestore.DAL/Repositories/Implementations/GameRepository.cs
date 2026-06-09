using Gamestore.DAL.Data;
using Gamestore.DAL.Entities;
using Gamestore.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.DAL.Repositories.Implementations;

public class GameRepository(GamestoreContext context) : IGameRepository
{
    public async Task<Game?> GetByKeyAsync(string key)
    {
        return await context.Games
            .Include(game => game.GameGenres)
            .Include(game => game.GamePlatforms)
            .FirstOrDefaultAsync(game => game.Key == key)!;
    }

    public async Task<Game?> GetByIdAsync(Guid id)
    {
        return await context.Games.FirstOrDefaultAsync(game => game.Id == id)!;
    }

    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        return await context.Games.ToListAsync()!;
    }

    public async Task<IEnumerable<Game>> GetByGenreAsync(Guid genreId)
    {
        return await context.Games
            .Where(game => game.GameGenres.Any(gamegenre => gamegenre.GenreId == genreId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Game>> GetByPlatformAsync(Guid platformId)
    {
        return await context.Games
            .Where(game => game.GamePlatforms.Any(gameplatform => gameplatform.PlatformId == platformId))
            .ToListAsync();
    }

    public async Task AddAsync(Game game)
    {
        await context.Games.AddAsync(game)!;
    }

    public Task UpdateAsync(Game game)
    {
        context.Games.Update(game);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Game game)
    {
        context.Games.Remove(game);
        return Task.CompletedTask;
    }

    public async Task<int> GetCountAsync()
    {
        return await context.Games.CountAsync();
    }
}