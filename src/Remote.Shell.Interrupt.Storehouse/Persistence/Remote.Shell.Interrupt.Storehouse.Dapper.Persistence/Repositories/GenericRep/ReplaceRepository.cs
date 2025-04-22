namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class ReplaceRepository<T>(PostgreSQLDapperContext context)
  : IReplaceRepository<T> where T : BaseEntity
{
  void IReplaceRepository<T>.ReplaceOne(T entity)
  {
    context.BeginTransaction();
    string updateProperties = GetUpdateProperties.Handle<T>(excludeKey: true);
    var baseQuery = $"UPDATE \"{GetTableName.Handle<T>()}\" SET {updateProperties} WHERE \"{nameof(BaseEntity.Id)}\"=@Id";
    var connection = context.CreateConnection();
    connection.Execute(baseQuery, entity);
  }
}
