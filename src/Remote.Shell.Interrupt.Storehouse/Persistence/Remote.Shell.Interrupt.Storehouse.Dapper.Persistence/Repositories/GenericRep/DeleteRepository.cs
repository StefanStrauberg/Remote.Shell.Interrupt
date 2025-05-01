namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class DeleteRepository<T>(PostgreSQLDapperContext context)
  : IDeleteRepository<T> where T : BaseEntity
{
  void IDeleteRepository<T>.DeleteOne(T entity)
  {
    context.BeginTransaction();

    var queryBuilder = new SqlQueryBuilder<T>();

    var sql = queryBuilder.BuildDelete(entity.Id);

    var connection = context.CreateConnection();

    connection.Execute(sql);
  }
}
