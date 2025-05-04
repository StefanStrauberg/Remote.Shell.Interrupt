namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class BulkInsertRepository<T>()
  : IBulkInsertRepository<T> where T : BaseEntity
{
  void IBulkInsertRepository<T>.InsertMany(IEnumerable<T> entities)
  {
    throw new NotImplementedException();
  }
}
