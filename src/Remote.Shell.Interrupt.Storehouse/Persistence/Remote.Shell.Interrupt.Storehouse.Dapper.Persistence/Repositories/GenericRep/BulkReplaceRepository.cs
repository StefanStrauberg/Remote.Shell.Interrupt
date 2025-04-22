namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class BulkReplaceRepository<T>(PostgreSQLDapperContext context,
                                        IReplaceRepository<T> replaceRepository)
  : IBulkReplaceRepository<T> where T : BaseEntity
{
  void IBulkReplaceRepository<T>.ReplaceMany(IEnumerable<T> entities)
  {
    context.BeginTransaction();
    foreach (var entity in entities)
    {
      replaceRepository.ReplaceOne(entity);
    }
  }
}
