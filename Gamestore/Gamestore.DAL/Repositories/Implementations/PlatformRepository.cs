using Gamestore.DAL.Data;
using Gamestore.DAL.Entities;
using Gamestore.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.DAL.Repositories.Implementations;

public class PlatformRepository: IPlatformRepository
{
    private readonly GamestoreContext _context;

    public PlatformRepository(GamestoreContext context)
    {
        _context = context;
    }

    public async Task<Platform?> GetByIdAsync(Guid id)
    {
        return await _context.Platforms.FirstOrDefaultAsync(platform => platform.Id == id);
    }

    public async Task<IEnumerable<Platform>> GetAllAsync()
    {
        return await _context.Platforms.ToListAsync();
    }
    
    public async Task<IEnumerable<Platform>> GetByGameKeyAsync(string gameKey)
    {
        return await _context.Platforms
            .Where(platform => platform.GamePlatforms.Any(gamePlatform => gamePlatform.Game.Key==gameKey))
            .ToListAsync(); 
    }

    public async Task AddAsync(Platform platform)
    {
        await _context.Platforms.AddAsync(platform);
    }

    public Task UpdateAsync(Platform platform)
    {
        _context.Platforms.Update(platform);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Platform platform)
    {
        _context.Platforms.Remove(platform);
        return Task.CompletedTask;
    }
    
}