namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class InsertRepository<T>(ApplicationDbContext context)
  : IInsertRepository<T> where T : BaseEntity
{
  readonly ApplicationDbContext _context = context;
  readonly DbSet<T> _dbSet = new(context.ModelBuilder, context);

  void IInsertRepository<T>.InsertOne(T entity)
  {
    using var connection = _context.GetConnection();
    var entityId = _dbSet.Insert(connection, entity);
    entity.Id = entityId;
  }
}