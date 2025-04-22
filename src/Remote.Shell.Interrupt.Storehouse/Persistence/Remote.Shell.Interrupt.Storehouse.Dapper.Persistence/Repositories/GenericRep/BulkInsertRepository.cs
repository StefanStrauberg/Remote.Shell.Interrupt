namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class BulkInsertRepository<T>(PostgreSQLDapperContext context,
                                       IInsertRepository<T> insertRepository)
  : IBulkInsertRepository<T> where T : BaseEntity
{
  void IBulkInsertRepository<T>.InsertMany(IEnumerable<T> entities)
  {
    context.BeginTransaction();
    foreach (var entity in entities)
    {
      insertRepository.InsertOne(entity);
    }
  }
}
