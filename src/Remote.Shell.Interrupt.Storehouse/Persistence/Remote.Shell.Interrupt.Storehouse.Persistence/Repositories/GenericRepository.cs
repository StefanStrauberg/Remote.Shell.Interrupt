namespace Remote.Shell.Interrupt.Storehouse.Persistence.Repositories;

internal class GenericRepository<T>(ApplicationDbContext dbContext)
  : IGenericRepository<T> where T : BaseEntity
{
  protected readonly DbSet<T> _dbSet = dbContext.Set<T>()
    ?? throw new ArgumentNullException(nameof(dbContext));
  protected readonly ApplicationDbContext _dbContext = dbContext
    ?? throw new ArgumentNullException(nameof(dbContext));

  void IGenericRepository<T>.DeleteMany(IEnumerable<T> entities)
    => _dbSet.RemoveRange(entities);

  void IGenericRepository<T>.DeleteOne(T entity)
    => _dbSet.Remove(entity);

  async Task<bool> IGenericRepository<T>.AnyAsync(Expression<Func<T, bool>> predicate,
                                                  CancellationToken cancellationToken)
    => await _dbSet.AsNoTracking()
                   .AnyAsync(predicate: predicate,
                             cancellationToken: cancellationToken);

  async Task<T> IGenericRepository<T>.FirstAsync(Expression<Func<T, bool>> predicate,
                                                 CancellationToken cancellationToken)
    => await _dbSet.AsNoTracking()
                   .FirstAsync(predicate: predicate,
                               cancellationToken: cancellationToken);

  async Task<IEnumerable<T>> IGenericRepository<T>.GetAllAsync(CancellationToken cancellationToken)
   => await _dbSet.AsNoTracking()
                  .ToListAsync(cancellationToken);

  void IGenericRepository<T>.InsertMany(IEnumerable<T> entities)
    => _dbSet.AddRange(entities);

  void IGenericRepository<T>.InsertOne(T entity)
    => _dbSet.Add(entity);

  void IGenericRepository<T>.ReplaceOne(T entity)
    => _dbSet.Update(entity);
}
