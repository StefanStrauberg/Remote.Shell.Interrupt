namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class BulkDeleteRepository<T>(PostgreSQLDapperContext context,
                                       IDeleteRepository<T> deleteRepository)
  : IBulkDeleteRepository<T> where T : BaseEntity
{
  public void DeleteMany(IEnumerable<T> entities)
  {
    context.BeginTransaction();
    foreach (var entity in entities)
    {
      deleteRepository.DeleteOne(entity);
    }
  }
}
