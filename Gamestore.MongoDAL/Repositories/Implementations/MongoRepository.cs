using System.Linq.Expressions;
using Gamestore.MongoDAL.Data;
using Gamestore.MongoDAL.Repositories.Interfaces;
using MongoDB.Driver;

namespace Gamestore.MongoDAL.Repositories.Implementations;

public class MongoRepository<T>(NorthwindMongoContext context, string collectionName) : IMongoRepository<T>
    where T : class
{
    protected IMongoCollection<T> Collection { get; } = context.Database.GetCollection<T>(collectionName);

    protected NorthwindMongoContext Context { get; } = context;

    public IQueryable<T> GetAllQueryable()
    {
        return Collection.AsQueryable();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        return await Collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Collection.Find(_ => true).ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await Collection.Find(predicate).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await Collection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(string id, T entity)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        await Collection.ReplaceOneAsync(filter, entity);
    }

    public Task DeleteAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        return Collection.DeleteOneAsync(filter);
    }
}