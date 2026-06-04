using Gamestore.DAL.Data;
using Gamestore.DAL.Repositories.Interfaces;

namespace Gamestore.DAL.UnitOfWork;

public class UnitOfWork(
    GamestoreContext context,
    IGameRepository games,
    IGenreRepository genres,
    IPlatformRepository platforms) : IUnitOfWork
{
    private readonly GamestoreContext _context = context;

    public IGameRepository Games { get; } = games;

    public IGenreRepository Genres { get; } = genres;

    public IPlatformRepository Platforms { get; } = platforms;

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}