using Gamestore.MongoDAL.Data;
using Gamestore.MongoDAL.Entities;
using Gamestore.MongoDAL.Repositories.Interfaces;
using MongoDB.Driver;

namespace Gamestore.MongoDAL.Repositories.Implementations;

public class MongoOrderRepository(NorthwindMongoContext context) : MongoRepository<MongoOrder>(context, "orders"), IMongoOrderRepository
{
    public async Task<IEnumerable<MongoOrder>> GetHistoryAsync(DateTime? startDate, DateTime? endDate)
    {
        var builder = Builders<MongoOrder>.Filter;
        var filter = builder.Empty;
        if (startDate.HasValue)
        {
            filter &= builder.Gte(o => o.OrderDate, startDate.Value);
        }

        if (endDate.HasValue)
        {
            filter &= builder.Lte(o => o.OrderDate, endDate.Value);
        }

        return await Collection.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<MongoOrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId)
    {
        var filter = Builders<MongoOrderDetail>.Filter.Eq(d => d.OrderID, orderId);
        return await Context.OrderDetails.Find(filter).ToListAsync();
    }
}
