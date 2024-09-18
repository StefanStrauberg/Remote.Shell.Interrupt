namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);

    Task<T> FindOneAsync(Expression<Func<T, bool>> filterExpression,
                         CancellationToken cancellationToken);

    Task InsertOneAsync(T document,
                        CancellationToken cancellationToken);

    Task InsertManyAsync(IEnumerable<T> documents,
                         CancellationToken cancellationToken);

    Task ReplaceOneAsync(T document,
                         CancellationToken cancellationToken);

    Task DeleteOneAsync(T document,
                        CancellationToken cancellationToken);

    Task DeleteManyAsync(Expression<Func<T, bool>> filterExpression,
                         CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Expression<Func<T, bool>> filterExpression,
                           CancellationToken cancellationToken);
}
