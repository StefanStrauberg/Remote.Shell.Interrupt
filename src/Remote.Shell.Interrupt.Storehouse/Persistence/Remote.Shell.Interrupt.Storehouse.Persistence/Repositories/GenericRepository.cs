namespace Remote.Shell.Interrupt.Storehouse.Persistence.Repositories;

internal class GenericRepository<TDocument>(ApplicationDbContext dbContext)
  : IGenericRepository<TDocument> where TDocument : BaseEntity
{
  protected readonly DbSet<TDocument> _dbSet = dbContext.Set<TDocument>()
    ?? throw new ArgumentNullException(nameof(dbContext));

  public virtual async Task DeleteManyAsync(System.Linq.Expressions.Expression<Func<TDocument, bool>> filterExpression,
                                            CancellationToken cancellationToken)
  {
    var documents = await _dbSet.Where(filterExpression)
                                .AsNoTracking()
                                .ToListAsync(cancellationToken);
    _dbSet.RemoveRange(documents);
    await dbContext.SaveChangesAsync(cancellationToken);
  }

  public virtual async Task DeleteOneAsync(TDocument document,
                                           CancellationToken cancellationToken)
  {
    _dbSet.Remove(document);
    await dbContext.SaveChangesAsync(cancellationToken);
  }

  public virtual async Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<TDocument, bool>> filterExpression,
                                              CancellationToken cancellationToken)
    => await _dbSet.AsNoTracking()
                   .AnyAsync(predicate: filterExpression,
                             cancellationToken: cancellationToken);

  public async virtual Task<TDocument> FindOneAsync(System.Linq.Expressions.Expression<Func<TDocument, bool>> filterExpression,
                                                    CancellationToken cancellationToken)
    => await _dbSet.AsNoTracking()
                  .FirstAsync(predicate: filterExpression,
                              cancellationToken: cancellationToken);

  public virtual async Task<IEnumerable<TDocument>> GetAllAsync(CancellationToken cancellationToken)
   => await _dbSet.AsNoTracking()
                  .ToListAsync(cancellationToken);

  public async virtual Task InsertManyAsync(IEnumerable<TDocument> documents,
                                            CancellationToken cancellationToken)
  {
    await _dbSet.AddRangeAsync(documents, cancellationToken);
    await dbContext.SaveChangesAsync(cancellationToken);
  }

  public virtual async Task InsertOneAsync(TDocument document,
                                           CancellationToken cancellationToken)
  {
    await _dbSet.AddAsync(document, cancellationToken);
    await dbContext.SaveChangesAsync(cancellationToken);
  }

  public virtual async Task ReplaceOneAsync(TDocument document,
                                            CancellationToken cancellationToken)
  {
    _dbSet.Update(document);
    await dbContext.SaveChangesAsync(cancellationToken);
  }
}
