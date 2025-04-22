namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class WriteOneRepository<T>(PostgreSQLDapperContext context)
  : IWriteOneRepository<T> where T : BaseEntity
{
  public void DeleteOne(T entity)
  {
    context.BeginTransaction();
    var baseQuery = $"DELETE FROM \"{GetTableName.Handle<T>()}\" WHERE \"{nameof(BaseEntity.Id)}\"=@Id";
    var connection = context.CreateConnection();
    connection.Execute(baseQuery, new { Id = entity.Id });
  }

  public void InsertOne(T entity)
  {
    var sb = new StringBuilder();
    sb.Append($"INSERT INTO \"{GetTableName.Handle<T>()}\" ");
    sb.Append($"({GetColumnsAsProperties.Handle<T>(excludeKey: true)}) ");
    sb.Append($"VALUES ({GetPropertyNames.Handle<T>(excludeKey: true)}) ");
    sb.Append($"RETURNING \"{nameof(BaseEntity.Id)}\"");
    var baseQuery = sb.ToString();

    context.BeginTransaction();
    var connection = context.CreateConnection();
    var entityId = connection.ExecuteScalar<Guid>(baseQuery, entity);
    entity.Id = entityId;
  }

  public void ReplaceOne(T entity)
  {
    context.BeginTransaction();
    string updateProperties = GetUpdateProperties.Handle<T>(excludeKey: true);
    var baseQuery = $"UPDATE \"{GetTableName.Handle<T>()}\" SET {updateProperties} WHERE \"{nameof(BaseEntity.Id)}\"=@Id";
    var connection = context.CreateConnection();
    connection.Execute(baseQuery, entity);
  }
}
