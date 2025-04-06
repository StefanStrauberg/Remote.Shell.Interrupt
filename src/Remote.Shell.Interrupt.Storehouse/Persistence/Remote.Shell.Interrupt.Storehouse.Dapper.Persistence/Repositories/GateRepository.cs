namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class GateRepository(PostgreSQLDapperContext context) 
    : GenericRepository<Gate>(context), IGateRepository
{
    async Task<IEnumerable<Gate>> IGateRepository.GetAllGatesAsync(RequestParameters requestParameters,
                                                                   CancellationToken cancellationToken)
    {
      StringBuilder sb = new();
      sb.Append($"SELECT g.\"{nameof(Gate.Id)}\", g.\"{nameof(Gate.Name)}\", g.\"{nameof(Gate.IPAddress)}\", ");
      sb.Append($"g.\"{nameof(Gate.Community)}\", g.\"{nameof(Gate.TypeOfNetworkDevice)}\" ");
      sb.Append($"FROM \"{GetTableName<Gate>()}\" AS g ");

      var baseQuery = sb.ToString();
      var queryBuilder = new SqlQueryBuilder(requestParameters,
                                             "g",
                                             typeof(Gate));
      var (finalQuery, parameters) = queryBuilder.BuildBaseQuery(baseQuery);

      var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
      var gates = await connection.QueryAsync<Gate>(finalQuery, parameters);

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
