using System.Linq.Expressions;

namespace FreeLink.Domain.Ports;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll();
    Task<T?> GetById(object id);
    Task Add(T entity);
    Task Update(T entity);
    Task Delete(object id);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate);
}