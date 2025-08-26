namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class BulkDeleteRepository<T>(ApplicationDbContext context)
  : IBulkDeleteRepository<T> where T : BaseEntity
{
  public void DeleteMany(IEnumerable<T> entities)
    => context.Set<T>().RemoveRange(entities);
}
