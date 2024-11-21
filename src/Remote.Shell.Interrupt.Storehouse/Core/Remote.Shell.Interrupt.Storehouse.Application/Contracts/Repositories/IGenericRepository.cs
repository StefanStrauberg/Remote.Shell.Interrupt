namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);

    Task<T> FirstByIdAsync(Guid id,
                           CancellationToken cancellationToken);

    void InsertOne(T entity);

    void InsertMany(IEnumerable<T> entities);

    void ReplaceOne(T entity);
    void ReplaceMany(IEnumerable<T> entities);

    void DeleteOne(T entity);

    void DeleteMany(IEnumerable<T> entities);

    Task<bool> AnyAsync(CancellationToken cancellationToken);
    Task<bool> AnyByIdAsync(Guid id,
                            CancellationToken cancellationToken);
}
