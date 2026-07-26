using Gamestore.DAL.Data;
using Gamestore.DAL.Entities;
using Gamestore.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.DAL.Repositories.Implementations;

public class OrderGameRepository(GamestoreContext context) : Repository<OrderGame>(context), IOrderGameRepository
{
    public Task<OrderGame?> GetByOrderAndProductAsync(Guid orderId, Guid productId)
    {
        return Context.OrderGames
            .FirstOrDefaultAsync(order => order.OrderId == orderId && order.ProductId == productId);
    }

    public async Task<IEnumerable<OrderGame>> GetByOrderIdAsync(Guid orderId)
    {
        return await Context.OrderGames
            .Where(order => order.OrderId == orderId)
            .ToListAsync();
    }
}