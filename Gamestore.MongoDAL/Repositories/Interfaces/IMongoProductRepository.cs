using Gamestore.MongoDAL.Entities;

namespace Gamestore.MongoDAL.Repositories.Interfaces;

public interface IMongoProductRepository : IMongoRepository<MongoProduct>
{
    Task<MongoProduct?> GetByGameStoreKeyAsync(string gameKey);

    Task UpdateGameStoreKeyAsync(string productId, string gameKey);

    Task DecrementUnitsInStockAsync(string gameKey, int quantity);

    Task IncrementViewsCountAsync(string productId);
}
