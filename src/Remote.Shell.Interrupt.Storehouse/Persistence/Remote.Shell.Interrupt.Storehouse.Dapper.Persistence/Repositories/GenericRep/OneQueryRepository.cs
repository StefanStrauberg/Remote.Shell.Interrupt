namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class OneQueryRepository<T>(ApplicationDbContext context)
  : IOneQueryRepository<T> where T : BaseEntity
{
  readonly ApplicationDbContext _context = context;
  readonly DbSet<T> _dbSet = new(context.ModelBuilder, context);

  async Task<T> IOneQueryRepository<T>.GetOneShortAsync(ISpecification<T> specification,
                                                        CancellationToken cancellationToken)
  {
    using var connection = await _context.GetConnectionAsync();
    var result = await _dbSet.Where(specification.Criterias!)
                             .FirstAsync(connection);
    return result;
  }
}
