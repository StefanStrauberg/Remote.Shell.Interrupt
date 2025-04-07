
namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class SPRVlansRepository(PostgreSQLDapperContext context) 
  : GenericRepository<SPRVlan>(context), ISPRVlansRepository
{
  async Task<SPRVlan> ISPRVlansRepository.GetSPRVlanByQueryAsync(RequestParameters requestParameters,
                                                                 CancellationToken cancellationToken)
  {
    StringBuilder sb = new();
    sb.Append("SELECT ");
    sb.Append($"sprvl.\"{nameof(SPRVlan.Id)}\", sprvl.\"{nameof(SPRVlan.IdClient)}\", sprvl.\"{nameof(SPRVlan.UseClient)}\", ");
    sb.Append($"sprvl.\"{nameof(SPRVlan.UseCOD)}\", sprvl.\"{nameof(SPRVlan.IdVlan)}\" ");
    sb.Append($"FROM \"{GetTableName<SPRVlan>()}\" AS sprvl ");

    var baseQuery = sb.ToString();
    var queryBuilder = new SqlQueryBuilder(requestParameters,
                                           "sprvl",
                                           typeof(SPRVlan));
    var (finalQuery, parameters) = queryBuilder.BuildBaseQuery(baseQuery);
    
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    return await connection.QuerySingleAsync<SPRVlan>(finalQuery, parameters);
  }

  async Task<bool> ISPRVlansRepository.AnyByVlanTagAsync(int vlanTag,
                                                         CancellationToken cancellationToken)
  {
    StringBuilder sb = new();
    sb.Append("SELECT COUNT(1) ");
    sb.Append($"FROM \"{GetTableName<SPRVlan>()}\" AS sprvl ");
    sb.Append($"WHERE sprvl.\"{nameof(SPRVlan.IdVlan)}\" = @Tag");

    var query = sb.ToString();
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var count = await connection.ExecuteScalarAsync<int>(query, new { Tag = vlanTag });
    
    return count > 0;
  }

  async Task<IEnumerable<SPRVlan>> ISPRVlansRepository.GetSPRVlansByQueryAsync(RequestParameters requestParameters,
                                                                           CancellationToken cancellationToken)
  {
    StringBuilder sb = new();
    sb.Append("SELECT ");
    sb.Append($"sprvl.\"{nameof(SPRVlan.Id)}\", sprvl.\"{nameof(SPRVlan.IdClient)}\", sprvl.\"{nameof(SPRVlan.UseClient)}\", ");
    sb.Append($"sprvl.\"{nameof(SPRVlan.UseCOD)}\", sprvl.\"{nameof(SPRVlan.IdVlan)}\" ");
    sb.Append($"FROM \"{GetTableName<SPRVlan>()}\" AS sprvl ");

    var baseQuery = sb.ToString();
    var queryBuilder = new SqlQueryBuilder(requestParameters,
                                           "sprvl",
                                           typeof(SPRVlan));
    var (finalQuery, parameters) = queryBuilder.BuildBaseQuery(baseQuery);
    
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    return await connection.QueryAsync<SPRVlan>(finalQuery, parameters);
  }
}
