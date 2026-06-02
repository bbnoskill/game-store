using Gamestore.DAL.Data;
using Gamestore.DAL.Entities;
using Gamestore.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.DAL.Repositories.Implementations;

public class GameRepository : IGameRepository
{
    private readonly GamestoreContext _context;

    public GameRepository(GamestoreContext context)
    {
        _context = context;
    }

    public async Task<Game?> GetByKeyAsync(string gameKey)
    {
        return await _context.Games
            .Include(game=>game.GameGenres)
            .Include(game=>game.GamePlatforms)
            .FirstOrDefaultAsync(game => game.Key == gameKey)!;   
    }

    public async Task<Game?> GetByIdAsync(Guid id)
    {
        return await _context.Games.FirstOrDefaultAsync(game => game.Id == id)!;
    }

    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        return await _context.Games.ToListAsync()!;
    }

    public async Task<IEnumerable<Game>> GetByGenreAsync(Guid genreId)
    {
        return await _context.Games
            .Where(game => game.GameGenres.Any(gamegenre => gamegenre.GenreId == genreId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Game>> GetByPlatformAsync(Guid platformId)
    {
        return await _context.Games
            .Where(game => game.GamePlatforms.Any(gameplatform => gameplatform.PlatformId == platformId))
            .ToListAsync();
    }

    public async Task AddAsync(Game game)
    {
        await _context.Games.AddAsync(game)!;
    }

    public Task UpdateAsync(Game game)
    {
        _context.Games.Update(game);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Game game)
    {
        _context.Games.Remove(game);
        return Task.CompletedTask;
    }
}