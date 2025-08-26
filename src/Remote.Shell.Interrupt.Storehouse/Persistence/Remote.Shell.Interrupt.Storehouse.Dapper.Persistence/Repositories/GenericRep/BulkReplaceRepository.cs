namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class BulkReplaceRepository<T>(ApplicationDbContext context)
  : IBulkReplaceRepository<T> where T : BaseEntity
{
  void IBulkReplaceRepository<T>.ReplaceMany(IEnumerable<T> entities)
    => context.Set<T>().UpdateRange(entities);
}
