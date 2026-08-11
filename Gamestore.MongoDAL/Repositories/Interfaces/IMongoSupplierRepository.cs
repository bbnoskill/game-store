using Gamestore.MongoDAL.Entities;

namespace Gamestore.MongoDAL.Repositories.Interfaces;

public interface IMongoSupplierRepository : IMongoRepository<MongoSupplier>
{
    Task<MongoSupplier?> GetByCompanyNameAsync(string companyName);
}
