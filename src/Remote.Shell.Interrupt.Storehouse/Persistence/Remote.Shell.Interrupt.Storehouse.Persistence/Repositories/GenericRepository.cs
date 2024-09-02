using System.Linq.Dynamic.Core.Tokenizer;

namespace Remote.Shell.Interrupt.Storehouse.Persistence.Repositories;

internal class GenericRepository<TDocument>(IDocumentSession session)
  : IGenericRepository<TDocument> where TDocument : BaseEntity
{
  readonly IDocumentSession _session = session
    ?? throw new ArgumentNullException(nameof(session));

  public virtual async Task DeleteManyAsync(System.Linq.Expressions.Expression<Func<TDocument, bool>> filterExpression,
                                            CancellationToken cancellationToken)
  {
    var documents = await _session.Query<TDocument>()
                                  .Where(predicate: filterExpression)
                                  .ToListAsync(token: cancellationToken);
    _session.Delete(documents);
    await _session.SaveChangesAsync(token: cancellationToken);
  }

  public virtual async Task DeleteOneAsync(System.Linq.Expressions.Expression<Func<TDocument, bool>> filterExpression,
                                           CancellationToken cancellationToken)
  {
    var document = await FindOneAsync(filterExpression, cancellationToken);
    if (document != null)
    {
      _session.Delete(document);
      await _session.SaveChangesAsync(cancellationToken);
    }
  }

  public virtual async Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<TDocument, bool>> filterExpression,
                                              CancellationToken cancellationToken)
    => await _session.Query<TDocument>()
                     .AnyAsync(predicate: filterExpression,
                               token: cancellationToken);

  public async virtual Task<TDocument?> FindOneAsync(System.Linq.Expressions.Expression<Func<TDocument, bool>> filterExpression,
                                                     CancellationToken cancellationToken)
    => await _session.Query<TDocument>()
                     .FirstOrDefaultAsync(predicate: filterExpression,
                                          token: cancellationToken);

  public virtual async Task<IReadOnlyList<TDocument>> GetAllAsync(CancellationToken cancellationToken)
   => await _session.Query<TDocument>()
                    .ToListAsync(token: cancellationToken);

  public async virtual Task InsertManyAsync(IEnumerable<TDocument> documents,
                                            CancellationToken cancellationToken)
  {
    _session.Store(entities: documents);
    await _session.SaveChangesAsync(token: cancellationToken);
  }

  public virtual async Task InsertOneAsync(TDocument document,
                                     CancellationToken cancellationToken)
  {
    _session.Store(entities: document);
    await _session.SaveChangesAsync(token: cancellationToken);
  }

  public virtual async Task ReplaceOneAsync(System.Linq.Expressions.Expression<Func<TDocument, bool>> filterExpression,
                                      TDocument document,
                                      CancellationToken cancellationToken)
  {
    var existing = await FindOneAsync(filterExpression: filterExpression,
                                      cancellationToken: cancellationToken);
    if (existing != null)
    {
      _session.Store(entities: document);
      await _session.SaveChangesAsync(token: cancellationToken);
    }
  }
}
