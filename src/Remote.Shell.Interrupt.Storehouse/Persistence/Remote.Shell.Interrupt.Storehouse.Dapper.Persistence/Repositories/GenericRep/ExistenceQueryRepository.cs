namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class ExistenceQueryRepository<T>(ApplicationDbContext context)
  : IExistenceQueryRepository<T> where T : BaseEntity
{
  readonly ApplicationDbContext _context = context;
  readonly DbSet<T> _dbSet = new(context.ModelBuilder, context);

  async Task<bool> IExistenceQueryRepository<T>.AnyByQueryAsync(ISpecification<T> specification,
                                                                CancellationToken cancellationToken)
  {
    using var connection = await _context.GetConnectionAsync();
    var result = await _dbSet.Where(specification.Criterias!)
                             .AnyAsync(connection);
    return result;
  }
}
