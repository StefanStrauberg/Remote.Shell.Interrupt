namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class CountRepository<T>(ApplicationDbContext context)
  : ICountRepository<T> where T : BaseEntity
{
  readonly ApplicationDbContext _context = context;
  readonly DbSet<T> _dbSet = new(context.ModelBuilder, context);

  async Task<int> ICountRepository<T>.GetCountAsync(ISpecification<T> specification,
                                                    CancellationToken cancellationToken)
  {
    using var connection = await _context.GetConnectionAsync();
    var result = await _dbSet.Select(x => x.Id)
                             .Where(specification.Criterias!)
                             .CountAsync(connection);
    return result;
  }
}
