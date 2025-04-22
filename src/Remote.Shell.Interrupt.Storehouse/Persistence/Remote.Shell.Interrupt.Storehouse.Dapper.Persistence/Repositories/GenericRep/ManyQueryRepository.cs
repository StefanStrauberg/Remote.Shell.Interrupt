namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class ManyQueryRepository<T>(PostgreSQLDapperContext context)
  : IManyQueryRepository<T> where T : BaseEntity
{
  async Task<IEnumerable<T>> IManyQueryRepository<T>.GetManyShortAsync(RequestParameters requestParameters,
                                                                       CancellationToken cancellationToken)
  {
    var baseQuery = $"SELECT {GetColumnsAsProperties.Handle<T>()} FROM \"{GetTableName.Handle<T>()}\"";

    var queryBuilder = new SqlQueryBuilder(requestParameters,
                                           "",
                                           typeof(T));

    var (finalQuery, parameters) = queryBuilder.BuildBaseQuery(baseQuery);

    var connection = await context.CreateConnectionAsync(cancellationToken);

    return await connection.QueryAsync<T>(finalQuery, parameters);
  }
}
