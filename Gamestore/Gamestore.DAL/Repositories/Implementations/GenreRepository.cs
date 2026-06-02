using Gamestore.DAL.Data;
using Gamestore.DAL.Entities;
using Gamestore.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.DAL.Repositories.Implementations;

public class GenreRepository : IGenreRepository
{
    private readonly GamestoreContext _context;

    public GenreRepository(GamestoreContext context)
    {
        _context = context;
    }

    public async Task<Genre?> GetByIdAsync(Guid id)
    {
        return await _context.Genres.FirstOrDefaultAsync(genre => genre.Id == id);
    }

    public async Task<IEnumerable<Genre>> GetAllAsync()
    {
        return await _context.Genres.ToListAsync();
    }

    public async Task<IEnumerable<Genre>> GetByGameKeyAsync(string gameKey)
    {
        return await _context.Genres
            .Where(genre => genre.GameGenres.Any(gameGenre => gameGenre.Game.Key==gameKey))
            .ToListAsync(); 
    }

    public async Task<IEnumerable<Genre>> GetByParentIdAsync(Guid parentId)
    {
        return await _context.Genres
            .Where(genre => genre.ParentGenreId == parentId).ToListAsync();
    }

    public async Task AddAsync(Genre genre)
    {
        await _context.Genres.AddAsync(genre);
    }   

    public Task UpdateAsync(Genre genre)
    {
        _context.Genres.Update(genre);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Genre genre)
    {
        _context.Genres.Remove(genre);
        return Task.CompletedTask;
    }
}