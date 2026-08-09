using Gamestore.MongoDAL.Data;
using Gamestore.MongoDAL.Entities;
using Gamestore.MongoDAL.Repositories.Interfaces;
using MongoDB.Driver;

namespace Gamestore.MongoDAL.Repositories.Implementations;

public class MongoProductRepository(NorthwindMongoContext context) : MongoRepository<MongoProduct>(context, "products"), IMongoProductRepository
{
    public async Task<MongoProduct?> GetByGameStoreKeyAsync(string gameKey)
    {
        var filter = Builders<MongoProduct>.Filter.Eq(p => p.GameStoreKey, gameKey);
        return await Collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task UpdateGameStoreKeyAsync(string productId, string gameKey)
    {
        var filter = Builders<MongoProduct>.Filter.Eq(p => p.Id, productId);
        var update = Builders<MongoProduct>.Update.Set(p => p.GameStoreKey, gameKey);
        await Collection.UpdateOneAsync(filter, update);
    }

    public async Task DecrementUnitsInStockAsync(string gameKey, int quantity)
    {
        short qty = (short)quantity;
        var filter = Builders<MongoProduct>.Filter.And(
            Builders<MongoProduct>.Filter.Eq(p => p.GameStoreKey, gameKey),
            Builders<MongoProduct>.Filter.Gte(p => p.UnitsInStock, qty));
        var update = Builders<MongoProduct>.Update.Inc(p => p.UnitsInStock, (short)-qty);
        await Collection.UpdateOneAsync(filter, update);
    }

    public async Task IncrementViewsCountAsync(string productId)
    {
        var filter = Builders<MongoProduct>.Filter.Eq(p => p.Id, productId);
        var update = Builders<MongoProduct>.Update.Inc(p => p.ViewsCount, 1);
        await Collection.UpdateOneAsync(filter, update);
    }
}
