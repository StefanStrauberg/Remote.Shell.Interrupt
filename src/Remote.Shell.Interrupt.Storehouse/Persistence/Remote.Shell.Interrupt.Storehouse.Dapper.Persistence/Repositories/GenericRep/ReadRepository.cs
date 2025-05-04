namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class ReadRepository<T>(ApplicationDbContext context)
  : IReadRepository<T> where T : BaseEntity
{
  readonly ApplicationDbContext _context = context;
  readonly DbSet<T> _dbSet = new(context.ModelBuilder, context);

  async Task<IEnumerable<T>> IReadRepository<T>.GetAllAsync(CancellationToken cancellationToken)
  {
    using var connection = await _context.GetConnectionAsync();
    var result = await _dbSet.ToListAsync(connection);
    return result;
  }
}
