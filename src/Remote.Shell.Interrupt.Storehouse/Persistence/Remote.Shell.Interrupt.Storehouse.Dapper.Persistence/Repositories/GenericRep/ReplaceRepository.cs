namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class ReplaceRepository<T>(ApplicationDbContext context)
  : IReplaceRepository<T> where T : BaseEntity
{
  readonly ApplicationDbContext _context = context;
  readonly DbSet<T> _dbSet = new(context.ModelBuilder, context);

  void IReplaceRepository<T>.ReplaceOne(T entity)
  {
    using var connection = _context.GetConnection();
    _dbSet.Update(connection, entity);
  }
}
