using Gamestore.MongoDAL.Entities;

namespace Gamestore.MongoDAL.Repositories.Interfaces;

public interface IMongoOrderRepository : IMongoRepository<MongoOrder>
{
    Task<IEnumerable<MongoOrder>> GetHistoryAsync(DateTime? startDate, DateTime? endDate);

    Task<IEnumerable<MongoOrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId);
}
