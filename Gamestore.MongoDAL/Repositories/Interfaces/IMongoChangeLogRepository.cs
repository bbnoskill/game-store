using Gamestore.MongoDAL.Entities;

namespace Gamestore.MongoDAL.Repositories.Interfaces;

public interface IMongoChangeLogRepository : IMongoRepository<MongoChangeLog>
{
    Task InsertLogRawAsync(MongoChangeLog log);

    Task<IEnumerable<MongoChangeLog>> GetLogsByEntityTypeAsync(string entityType);
}
