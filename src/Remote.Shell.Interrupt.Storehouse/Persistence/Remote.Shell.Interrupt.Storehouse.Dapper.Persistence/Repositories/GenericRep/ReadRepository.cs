namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class ReadRepository<T>(PostgreSQLDapperContext context)
  : IReadRepository<T> where T : BaseEntity
{
  async Task<IEnumerable<T>> IReadRepository<T>.GetAllAsync(CancellationToken cancellationToken)
  {
    var query = $"SELECT {GetColumnsAsProperties.Handle<T>()} FROM \"{GetTableName.Handle<T>()}\"";
    
    var connection = context.CreateConnection();

    return await connection.QueryAsync<T>(query);
  }
}
