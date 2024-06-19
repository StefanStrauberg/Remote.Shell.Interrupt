namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts;

public interface IGenericRepository<TDocument> where TDocument : BaseEntity
{
  Task<List<TDocument>> GetAllAsync(CancellationToken cancellationToken);

  Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression,
                               CancellationToken cancellationToken);

  Task InsertOneAsync(TDocument document,
                      CancellationToken cancellationToken);

  Task InsertManyAsync(IEnumerable<TDocument> documents,
                       CancellationToken cancellationToken);

  Task ReplaceOneAsync(Expression<Func<TDocument, bool>> filterExpression,
                       TDocument document,
                       CancellationToken cancellationToken);

  Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression,
                      CancellationToken cancellationToken);

  Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression,
                       CancellationToken cancellationToken);

  Task<bool> ExistsAsync(Expression<Func<TDocument, bool>> filterExpression,
                         CancellationToken cancellationToken);
}
