using Gamestore.DAL.Data;
using Gamestore.DAL.Repositories.Interfaces;

namespace Gamestore.DAL.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly GamestoreContext _context;

    public IGameRepository Games { get; }
    public IGenreRepository Genres { get; }
    public IPlatformRepository Platforms { get; }

    public UnitOfWork(GamestoreContext context,
        IGameRepository games,
        IGenreRepository genres,
        IPlatformRepository platforms)
    {
        _context = context;
        Games = games;
        Genres = genres;
        Platforms = platforms;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

}