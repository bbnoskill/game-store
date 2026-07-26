using Gamestore.DAL.Data;
using Gamestore.DAL.Repositories.Implementations;
using Gamestore.DAL.Repositories.Interfaces;

namespace Gamestore.DAL.UnitOfWork;

public class UnitOfWork(GamestoreContext context) : IUnitOfWork
{
    public IGameRepository Games { get; } = new GameRepository(context);

    public IGenreRepository Genres { get; } = new GenreRepository(context);

    public IPlatformRepository Platforms { get; } = new PlatformRepository(context);

    public IPublisherRepository Publishers { get; } = new PublisherRepository(context);

    public IOrderRepository Orders { get; } = new OrderRepository(context);

    public IOrderGameRepository OrderGames { get; } = new OrderGameRepository(context);

    public ICommentRepository Comments { get; } = new CommentRepository(context);

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}