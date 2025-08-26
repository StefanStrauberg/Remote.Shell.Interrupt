namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class BulkInsertRepository<T>(ApplicationDbContext context)
  : IBulkInsertRepository<T> where T : BaseEntity
{
  void IBulkInsertRepository<T>.InsertMany(IEnumerable<T> entities)
    => context.Set<T>().AddRange(entities);
}
