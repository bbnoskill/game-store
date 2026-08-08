using Gamestore.MongoDAL.Entities;

namespace Gamestore.MongoDAL.Repositories.Interfaces;

public interface IMongoCategoryRepository : IMongoRepository<MongoCategory>
{
    Task<MongoCategory?> GetByNameAsync(string categoryName);
}
