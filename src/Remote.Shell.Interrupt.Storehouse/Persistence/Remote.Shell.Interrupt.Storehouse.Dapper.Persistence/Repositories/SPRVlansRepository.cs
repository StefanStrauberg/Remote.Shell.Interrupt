namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class SPRVlansRepository(PostgreSQLDapperContext context) 
  : GenericRepository<SPRVlan>(context), ISPRVlansRepository
{
  async Task<IEnumerable<SPRVlan>> ISPRVlansRepository.GetAllByClientIdsAsync(IEnumerable<int> ids,
                                                                              CancellationToken cancellationToken)
  {
    StringBuilder sb = new();
    sb.Append("SELECT ");
    sb.Append($"sprvl.\"{nameof(SPRVlan.Id)}\", sprvl.\"{nameof(SPRVlan.IdClient)}\", sprvl.\"{nameof(SPRVlan.UseClient)}\", sprvl.\"{nameof(SPRVlan.UseCOD)}\", sprvl.\"{nameof(SPRVlan.IdVlan)}\" ");
    sb.Append($"FROM \"{GetTableName<SPRVlan>()}\" AS sprvl ");
    sb.Append($"WHERE sprvl.\"{nameof(SPRVlan.IdClient)}\" IN ({ConvertIdsToString(ids)})");
    
    var query = sb.ToString();
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.QueryAsync<SPRVlan>(query);
    
    return result;
  }

  static string ConvertIdsToString(IEnumerable<int> ids)
    => string.Join(", ", ids);

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

  async Task<IEnumerable<int>> ISPRVlansRepository.GetClientsIdsByVlantTag(int vlanTag,
                                                                           CancellationToken cancellationToken)
  {
    StringBuilder sb = new();
    sb.Append("SELECT ");
    sb.Append($"sprvl.\"{nameof(SPRVlan.IdClient)}\" ");
    sb.Append($"FROM \"{GetTableName<SPRVlan>()}\" AS sprvl ");
    sb.Append($"WHERE sprvl.\"{nameof(SPRVlan.IdVlan)}\" = @Tag");
    
    var query = sb.ToString();
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.QueryAsync<int>(query, new { Tag = vlanTag });
    
    return result;
  }
}
