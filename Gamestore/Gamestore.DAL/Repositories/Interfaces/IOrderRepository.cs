using Gamestore.DAL.Entities;

namespace Gamestore.DAL.Repositories.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetOpenOrderByCustomerIdAsync(Guid customerId);

    Task<IEnumerable<Order>> GetCompletedOrdersAsync();

    Task<Order?> GetByIdWithDetailsAsync(Guid id);
}