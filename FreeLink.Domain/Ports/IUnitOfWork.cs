namespace FreeLink.Domain.Ports;

public interface IUnitOfWork : IDisposable
{
    IRepository<TEntity> Repository<TEntity>() where TEntity : class;
    
    // Aquí puedes agregar repositorios específicos cuando los necesites
    
    Task<int> Complete();
}