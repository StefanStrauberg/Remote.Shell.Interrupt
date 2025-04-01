namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class SPRVlansRepository(PostgreSQLDapperContext context) : GenericRepository<SPRVlan>(context), ISPRVlansRepository
{
  async Task<IEnumerable<SPRVlan>> ISPRVlansRepository.GetAllByClientIdsAsync(IEnumerable<int> ids,
                                                                              CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "vl.\"Id\", vl.\"IdClient\", vl.\"UseClient\", vl.\"UseCOD\", vl.\"IdVlan\" " +
                $"FROM \"{GetTableName<SPRVlan>()}\" AS vl " +
                $"WHERE vl.\"IdClient\" IN ({ConvertIdsToString(ids)})";
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.QueryAsync<SPRVlan>(query);
    return result;
  }

  static string ConvertIdsToString(IEnumerable<int> ids)
    => string.Join(", ", ids);

  async Task<bool> ISPRVlansRepository.AnyByVlanTagAsync(int vlanTag,
                                                         CancellationToken cancellationToken)
  {
    var query = "SELECT COUNT(1) " +
                $"FROM \"{GetTableName<SPRVlan>()}\" AS vl " +
                $"WHERE vl.\"IdVlan\" = @Tag";
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var count = await connection.ExecuteScalarAsync<int>(query, new { Tag = vlanTag });
    return count > 0;
  }

  async Task<IEnumerable<int>> ISPRVlansRepository.GetClientsIdsByVlantTag(int vlanTag,
                                                                           CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "vl.\"IdClient\" " +
                $"FROM \"{GetTableName<SPRVlan>()}\" AS vl " +
                $"WHERE vl.\"IdVlan\" = @Tag";
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.QueryAsync<int>(query, new { Tag = vlanTag });
    return result;
  }
}
