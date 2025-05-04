namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class BulkReplaceRepository<T>()
  : IBulkReplaceRepository<T> where T : BaseEntity
{
  void IBulkReplaceRepository<T>.ReplaceMany(IEnumerable<T> entities)
  {
    throw new NotImplementedException();
  }
}
