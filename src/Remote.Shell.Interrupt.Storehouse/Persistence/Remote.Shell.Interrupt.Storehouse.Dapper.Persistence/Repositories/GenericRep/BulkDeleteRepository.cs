namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class BulkDeleteRepository<T>(PostgreSQLDapperContext context,
                                       IAppLogger<BulkDeleteRepository<T>> logger)
  : IBulkDeleteRepository<T> where T : BaseEntity
{
  public void DeleteMany(IEnumerable<T> entities)
  {
    context.BeginTransaction();

    var queryBuilder = new SqlQueryBuilder<T>();

    var sql = queryBuilder.BuildDeleteMany(entities.Select(x => x.Id));

    logger.LogInformation(sql);

    var connection = context.CreateConnection();

    connection.Execute(sql);
  }
}
