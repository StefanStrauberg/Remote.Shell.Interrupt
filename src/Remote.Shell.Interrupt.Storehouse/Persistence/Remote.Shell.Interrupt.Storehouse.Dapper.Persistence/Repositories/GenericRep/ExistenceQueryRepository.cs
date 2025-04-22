namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class ExistenceQueryRepository<T>(PostgreSQLDapperContext context)
  : IExistenceQueryRepository<T> where T : BaseEntity
{
  async Task<bool> IExistenceQueryRepository<T>.AnyByQueryAsync(RequestParameters requestParameters,
                                                                CancellationToken cancellationToken)
  {
    var baseQuery = $"SELECT COUNT(1) FROM \"{GetTableName.Handle<T>()}\"";

    var queryBuilder = new SqlQueryBuilder(requestParameters,
                                           typeof(T));

    var connection = await context.CreateConnectionAsync(cancellationToken);
    
    if (HasFiltersOrSorts.Handle(requestParameters))
    {
      var (finalQuery, parameters) = queryBuilder.BuildBaseQuery(baseQuery, true);

      return await connection.ExecuteScalarAsync<int>(finalQuery, parameters) > 0;
    }
    
    return await connection.ExecuteScalarAsync<int>(baseQuery) > 0;
  }
}
