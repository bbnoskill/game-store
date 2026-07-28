using Gamestore.DAL.Repositories.Interfaces;

namespace Gamestore.DAL.UnitOfWork;

public interface IUnitOfWork
{
    IGameRepository Games { get; }

    IGenreRepository Genres { get; }

    IPlatformRepository Platforms { get; }

    IPublisherRepository Publishers { get; }

    IOrderRepository Orders { get; }

    IOrderGameRepository OrderGames { get; }

    ICommentRepository Comments { get; }

    Task SaveChangesAsync();
}