using System.Linq.Expressions;
using FreeLink.Domain.Ports;
using FreeLink.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FreeLink.Infrastructure.Adapters;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly FreeLinkContext _context;

    public Repository(FreeLinkContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<T>> GetAll()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<T?> GetById(object id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task Add(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
    }

    public async Task Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }

    public async Task Delete(object id)
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity != null)
        {
            _context.Set<T>().Remove(entity);
        }
    }
    
    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        // Esto se traduce en: SELECT CASE WHEN EXISTS (SELECT 1 FROM Users WHERE Email = '...')
        return await _context.Set<T>().AnyAsync(predicate);
    }

    public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        // Esto se traduce en: SELECT TOP 1 * FROM Users WHERE Email = '...'
        return await _context.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate)
    {
        // Esto se traduce en: SELECT * FROM T WHERE [condición]
        return await _context.Set<T>().Where(predicate).ToListAsync();
    }
}