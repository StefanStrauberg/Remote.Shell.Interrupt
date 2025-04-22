namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class InsertRepository<T>(PostgreSQLDapperContext context)
  : IInsertRepository<T> where T : BaseEntity
{
  void IInsertRepository<T>.InsertOne(T entity)
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
}