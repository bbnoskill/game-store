using Gamestore.MongoDAL.Data;
using Gamestore.MongoDAL.Entities;
using Gamestore.MongoDAL.Repositories.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Gamestore.MongoDAL.Repositories.Implementations;

public class MongoChangeLogRepository(NorthwindMongoContext context) : MongoRepository<MongoChangeLog>(context, "changeLogs"), IMongoChangeLogRepository
{
    public async Task InsertLogRawAsync(MongoChangeLog log)
    {
        var json = log.ToJson();
        var command = new BsonDocumentCommand<BsonDocument>(
            BsonDocument.Parse($"{{ insert: 'changeLogs', documents: [{json}] }}"));
        await Context.Database.RunCommandAsync(command);
    }

    public async Task<IEnumerable<MongoChangeLog>> GetLogsByEntityTypeAsync(string entityType)
    {
        var filter = Builders<MongoChangeLog>.Filter.Eq(l => l.EntityType, entityType);
        return await Collection.Find(filter).ToListAsync();
    }
}
