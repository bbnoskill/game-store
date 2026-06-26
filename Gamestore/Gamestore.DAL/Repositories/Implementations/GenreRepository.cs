using Gamestore.DAL.Data;
using Gamestore.DAL.Entities;
using Gamestore.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.DAL.Repositories.Implementations;

public class GenreRepository(GamestoreContext context) : Repository<Genre>(context), IGenreRepository
{
    public async Task<IEnumerable<Genre>> GetByGameKeyAsync(string gameKey)
    {
        return await Context.Genres
            .Where(genre => genre.GameGenres.Any(gameGenre => gameGenre.Game.Key == gameKey))
            .ToListAsync();
    }

    public async Task<IEnumerable<Genre>> GetByParentIdAsync(Guid parentId)
    {
        return await Context.Genres
            .Where(genre => genre.ParentGenreId == parentId).ToListAsync();
    }
}