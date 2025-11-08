using System.Collections;
using FreeLink.Domain.Ports;
using FreeLink.Infrastructure.Data.Context;

namespace FreeLink.Infrastructure.Adapters;

public class UnitOfWork : IUnitOfWork
{
    private Hashtable? _repositories;
    private readonly FreeLinkContext _context;

    // Aquí puedes agregar repositorios específicos cuando los crees
    // private IUserRepository? _userRepository;
    // private IProjectRepository? _projectRepository;

    public UnitOfWork(FreeLinkContext context)
    {
        _context = context;
        _repositories = new Hashtable();
    }

    // Propiedades para repositorios específicos (agregar cuando sea necesario)

    public Task<int> Complete()
    {
        return _context.SaveChangesAsync();
    }

    public IRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        var type = typeof(TEntity).Name;

        if (_repositories != null && _repositories.ContainsKey(type))
        {
            return (IRepository<TEntity>)_repositories[type]!;
        }

        var repositoryType = typeof(Repository<>);
        var repositoryInstance = Activator.CreateInstance(
            repositoryType.MakeGenericType(typeof(TEntity)),
            _context
        );

        if (repositoryInstance != null)
        {
            _repositories?.Add(type, repositoryInstance);
            return (IRepository<TEntity>)repositoryInstance;
        }

        throw new Exception($"Unable to create repository for {type}");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}