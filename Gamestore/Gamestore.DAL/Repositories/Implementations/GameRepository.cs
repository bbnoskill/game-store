using Gamestore.DAL.Data;
using Gamestore.DAL.Entities;
using Gamestore.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.DAL.Repositories.Implementations;

public class GameRepository(GamestoreContext context) : Repository<Game>(context), IGameRepository
{
    public Task<LocalizedGameInfo?> GetLocalizedInfoAsync(string key, string languageCode)
    {
        return Context.Database.SqlQuery<LocalizedGameInfo>(
            $"SELECT Id, [Key], [Name], [Description], Price, Discount, UnitInStock FROM dbo.GetGameInfoByLocalization({key}, {languageCode})")
            .SingleOrDefaultAsync();
    }

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

    public async Task<IEnumerable<Game>> GetByPublisherAsync(string companyName)
    {
        return await Context.Games
            .Where(game => game.Publisher != null && (game.Publisher.CompanyName == companyName || game.Publisher.Localizations.Any(l => l.CompanyName == companyName)))
            .ToListAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await Context.Games.CountAsync();
    }

    public IQueryable<Game> GetQueryable()
    {
        return Context.Games
            .Include(g => g.GameGenres)
            .Include(g => g.GamePlatforms)
            .Include(g => g.GameComments);
    }
}