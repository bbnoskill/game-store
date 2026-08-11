using Gamestore.MongoDAL.Data;
using Gamestore.MongoDAL.Entities;
using Gamestore.MongoDAL.Repositories.Interfaces;
using MongoDB.Driver;

namespace Gamestore.MongoDAL.Repositories.Implementations;

public class MongoCategoryRepository(NorthwindMongoContext context) : MongoRepository<MongoCategory>(context, "categories"), IMongoCategoryRepository
{
    public async Task<MongoCategory?> GetByNameAsync(string categoryName)
    {
        var filter = Builders<MongoCategory>.Filter.Eq(c => c.CategoryName, categoryName);
        return await Collection.Find(filter).SingleOrDefaultAsync();
    }
}
