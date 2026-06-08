using Gamestore.DAL.Data;
using Gamestore.DAL.Entities;
using Gamestore.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.DAL.Repositories.Implementations;

public class GenreRepository(GamestoreContext context) : IGenreRepository
{
    public async Task<Genre?> GetByIdAsync(Guid id)
    {
        return await context.Genres.FirstOrDefaultAsync(genre => genre.Id == id);
    }

    public async Task<IEnumerable<Genre>> GetAllAsync()
    {
        return await context.Genres.ToListAsync();
    }

    public async Task<IEnumerable<Genre>> GetByGameKeyAsync(string gameKey)
    {
        return await context.Genres
            .Where(genre => genre.GameGenres.Any(gameGenre => gameGenre.Game.Key == gameKey))
            .ToListAsync();
    }

    public async Task<IEnumerable<Genre>> GetByParentIdAsync(Guid parentId)
    {
        return await context.Genres
            .Where(genre => genre.ParentGenreId == parentId).ToListAsync();
    }

    public async Task AddAsync(Genre genre)
    {
        await context.Genres.AddAsync(genre);
    }

    public Task UpdateAsync(Genre genre)
    {
        context.Genres.Update(genre);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Genre genre)
    {
        context.Genres.Remove(genre);
        return Task.CompletedTask;
    }
}