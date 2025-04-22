namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class CountRepository<T>(PostgreSQLDapperContext context)
  : ICountRepository<T> where T : BaseEntity
{
  async Task<int> ICountRepository<T>.GetCountAsync(RequestParameters requestParameters,
                                                    CancellationToken cancellationToken)
  {
    var baseQuery = $"SELECT COUNT(\"{nameof(BaseEntity.Id)}\") FROM \"{GetTableName.Handle<T>()}\"";
    
    var queryBuilder = new SqlQueryBuilder(requestParameters,
                                           typeof(T));

    var connection = await context.CreateConnectionAsync(cancellationToken);
    
    if (HasFiltersOrSorts.Handle(requestParameters))
    {
      var (finalQuery, parameters) = queryBuilder.BuildBaseQuery(baseQuery, true);

      return await connection.ExecuteScalarAsync<int>(finalQuery, parameters);
    }
    
    return await connection.ExecuteScalarAsync<int>(baseQuery);
  }
}
