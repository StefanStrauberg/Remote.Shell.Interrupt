namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class BulkDeleteRepository<T>()
  : IBulkDeleteRepository<T> where T : BaseEntity
{
  public void DeleteMany(IEnumerable<T> entities)
  {
    throw new NotImplementedException();
  }
}
