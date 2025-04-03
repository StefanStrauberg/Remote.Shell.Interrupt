namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class GateRepository(PostgreSQLDapperContext context) 
    : GenericRepository<Gate>(context), IGateRepository
{
    async Task<IEnumerable<Gate>> IGateRepository.GetAllAsync(RequestParameters requestParameters,
                                                              CancellationToken cancellationToken)
    {
      string columns = GetColumnsAsProperties();
      var offset = (requestParameters.PageNumber - 1) * requestParameters.PageSize;
      StringBuilder sb = new();
      sb.Append($"SELECT {columns} ");
      sb.Append($"FROM \"{GetTableName<Gate>()}\" ");
      sb.Append($"LIMIT {requestParameters.PageSize} OFFSET {offset}");

      var query = sb.ToString();
      var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
      var gates = await connection.QueryAsync<Gate>(query);

      return gates;
    }

    async Task<bool> IGateRepository.AnyByIPAddressAsync(string iPAddress,
                                                         CancellationToken cancellationToken)
    {
      StringBuilder sb = new();
      sb.Append("SELECT COUNT(1) ");
      sb.Append($"FROM \"{GetTableName<Gate>()}\" ");
      sb.Append($"WHERE \"{nameof(Gate.IPAddress)}\"=@IPAddress");

      var query = sb.ToString();
      var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
      var count = await connection.ExecuteScalarAsync<int>(query, new { IPAddress = iPAddress });
      
      return count > 0;
    }
}
