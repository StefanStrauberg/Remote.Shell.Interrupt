namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class SPRVlanLsRepository(PostgreSQLDapperContext context) : GenericRepository<SPRVlanL>(context), ISPRVlanLsRepository
{
  async Task<IEnumerable<SPRVlanL>> ISPRVlanLsRepository.GetAllByIdsAsync(IEnumerable<int> ids,
                                                                          CancellationToken cancellationToken)
  {
    string tableName = GetTableName();
    var query = $"SELECT " +
                "vl.\"Id\", vl.\"IdClient\", vl.\"UseClient\", vl.\"UseCOD\", vl.\"IdVlan\" " +
                $"FROM {tableName} AS vl " +
                $"WHERE vl.\"IdClient\" IN ({ConvertIdsToString(ids)})";
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.QueryAsync<SPRVlanL>(query);
    return result;
  }

  static string ConvertIdsToString(IEnumerable<int> ids)
    => string.Join(", ", ids);

  async Task<bool> ISPRVlanLsRepository.AnyByVlanTagAsync(int vlanTag,
                                                          CancellationToken cancellationToken)
  {
    string tableName = GetTableName();
    var query = "SELECT COUNT(1) " +
                $"FROM {tableName} AS vl " +
                $"WHERE vl.\"IdVlan\" = @Tag";
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var count = await connection.ExecuteScalarAsync<int>(query, new { Tag = vlanTag });
    return count > 0;
  }

  async Task<IEnumerable<int>> ISPRVlanLsRepository.GetClientsIdsByVlantTag(int vlanTag,
                                                                            CancellationToken cancellationToken)
  {
    string tableName = GetTableName();
    var query = $"SELECT " +
                "vl.\"IdClient\" " +
                $"FROM {tableName} AS vl " +
                $"WHERE vl.\"IdVlan\" = @Tag";
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.QueryAsync<int>(query, new { Tag = vlanTag });
    return result;
  }
}
