namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IGenericRepository<TDocument> where TDocument : BaseEntity
{
    Task<IEnumerable<TDocument>> GetAllAsync(CancellationToken cancellationToken);

    Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression,
                                 CancellationToken cancellationToken);

    Task InsertOneAsync(TDocument document,
                        CancellationToken cancellationToken);

    Task InsertManyAsync(IEnumerable<TDocument> documents,
                         CancellationToken cancellationToken);

    Task ReplaceOneAsync(TDocument document,
                         CancellationToken cancellationToken);

    Task DeleteOneAsync(TDocument document,
                        CancellationToken cancellationToken);

    Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression,
                         CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Expression<Func<TDocument, bool>> filterExpression,
                           CancellationToken cancellationToken);
}
