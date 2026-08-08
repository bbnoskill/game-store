using Gamestore.MongoDAL.Data;
using Gamestore.MongoDAL.Entities;
using Gamestore.MongoDAL.Repositories.Interfaces;
using MongoDB.Driver;

namespace Gamestore.MongoDAL.Repositories.Implementations;

public class MongoSupplierRepository(NorthwindMongoContext context) : MongoRepository<MongoSupplier>(context, "suppliers"), IMongoSupplierRepository
{
    public async Task<MongoSupplier?> GetByCompanyNameAsync(string companyName)
    {
        var filter = Builders<MongoSupplier>.Filter.Eq(s => s.CompanyName, companyName);
        return await Collection.Find(filter).SingleOrDefaultAsync();
    }
}
