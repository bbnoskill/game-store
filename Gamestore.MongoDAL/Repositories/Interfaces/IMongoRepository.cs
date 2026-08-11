using System.Linq.Expressions;

namespace Gamestore.MongoDAL.Repositories.Interfaces;

public interface IMongoRepository<T>
{
    IQueryable<T> GetAllQueryable();

    Task<T?> GetByIdAsync(Guid id);

    Task<IEnumerable<T>> GetAllAsync();

    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    Task AddAsync(T entity);

    Task UpdateAsync(string id, T entity);

    Task DeleteAsync(string id);
}