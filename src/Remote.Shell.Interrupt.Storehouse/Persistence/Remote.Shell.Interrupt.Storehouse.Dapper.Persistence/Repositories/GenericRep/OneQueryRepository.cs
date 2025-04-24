namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class OneQueryRepository<T>(PostgreSQLDapperContext context)
  : IOneQueryRepository<T> where T : BaseEntity
{
  async Task<T> IOneQueryRepository<T>.GetOneShortAsync(RequestParameters requestParameters,
                                                        CancellationToken cancellationToken)
  {
    var baseQuery = $"SELECT {GetColumnsAsProperties.Handle<T>()} FROM \"{GetTableName.Handle<T>()}\"";

    var queryBuilder = new SqlQueryBuilder(requestParameters,
                                           "",
                                           typeof(T));

    var (finalQuery, parameters) = queryBuilder.BuildBaseQuery(baseQuery, true);

    var connection = await context.CreateConnectionAsync(cancellationToken);

    return await connection.QuerySingleAsync<T>(finalQuery, parameters);
  }
}
