namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class ManyQueryRepository<T>(ApplicationDbContext context)
  : IManyQueryRepository<T> where T : BaseEntity
{
  readonly ApplicationDbContext _context = context;
  readonly DbSet<T> _dbSet = new(context.ModelBuilder, context);

  async Task<IEnumerable<T>> IManyQueryRepository<T>.GetManyShortAsync(ISpecification<T> specification,
                                                                       CancellationToken cancellationToken)
  {
    using var connection = await _context.GetConnectionAsync();
    var result = await _dbSet.Where(specification.Criterias!)
                             .Skip(specification.Skip)
                             .Take(specification.Take)
                             .ToListAsync(connection);
    return result;
  }
}
