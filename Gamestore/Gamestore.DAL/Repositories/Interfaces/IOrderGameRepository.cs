using Gamestore.DAL.Entities;

namespace Gamestore.DAL.Repositories.Interfaces;

public interface IOrderGameRepository : IRepository<OrderGame>
{
    Task<OrderGame?> GetByOrderAndProductAsync(Guid orderId, Guid productId);

    Task<IEnumerable<OrderGame>> GetByOrderIdAsync(Guid orderId);
}