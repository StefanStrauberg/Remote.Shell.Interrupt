namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class DeleteRepository<T>(ApplicationDbContext context)
  : IDeleteRepository<T> where T : BaseEntity
{
  readonly ApplicationDbContext _context = context;
  readonly DbSet<T> _dbSet = new(context.ModelBuilder, context);
  
  void IDeleteRepository<T>.DeleteOne(T entity)
  {
    using var connection = _context.GetConnection();
    _dbSet.Where(x => x.Id == entity.Id).Delete(connection);
  }
}
