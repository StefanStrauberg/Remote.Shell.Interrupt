namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);

    Task<T> FirstAsync(Expression<Func<T, bool>> predicate,
                       CancellationToken cancellationToken);

    void InsertOne(T entity);

    void InsertMany(IEnumerable<T> entities);

    void ReplaceOne(T entity);

    void DeleteOne(T entity);

    void DeleteMany(IEnumerable<T> entities);

    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate,
                        CancellationToken cancellationToken);
}
