using Gamestore.DAL.Data;
using Gamestore.DAL.Entities;
using Gamestore.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gamestore.DAL.Repositories.Implementations;

public class OrderRepository(GamestoreContext context) : Repository<Order>(context), IOrderRepository
{
    public Task<Order?> GetOpenOrderByCustomerIdAsync(Guid customerId)
    {
        return Context.Orders.FirstOrDefaultAsync(order => order.CustomerId == customerId && order.Status == OrderStatus.Open);
    }

    public async Task<IEnumerable<Order>> GetCompletedOrdersAsync()
    {
        return await Context.Orders
            .Where(order => order.Status == OrderStatus.Paid || order.Status == OrderStatus.Cancelled)
            .ToListAsync();
    }

    public Task<Order?> GetByIdWithDetailsAsync(Guid id)
    {
        return Context.Orders
            .Include(order => order.OrderGames)
            .FirstOrDefaultAsync(order => order.Id == id);
    }
}