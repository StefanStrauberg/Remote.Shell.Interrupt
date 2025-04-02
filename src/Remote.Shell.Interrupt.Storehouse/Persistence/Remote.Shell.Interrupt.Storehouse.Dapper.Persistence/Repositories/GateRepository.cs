using Remote.Shell.Interrupt.Storehouse.Application.Helper;

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class GateRepository(PostgreSQLDapperContext context) : GenericRepository<Gate>(context), IGateRepository
{
    async Task<IEnumerable<Gate>> IGateRepository.GetAllAsync(RequestParameters requestParameters,
                                                              CancellationToken cancellationToken)
    {
      string columns = GetColumnsAsProperties();
      var offset = (requestParameters.PageNumber - 1) * requestParameters.PageSize;
      var query = $"SELECT {columns} " +
                  $"FROM \"{GetTableName<Gate>()}\" " +
                  $"LIMIT {requestParameters.PageSize} OFFSET {offset}";;
      var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
      var gates = await connection.QueryAsync<Gate>(query);
      return gates;
    }

    async Task<bool> IGateRepository.AnyByIPAddressAsync(string iPAddress,
                                                         CancellationToken cancellationToken)
    {
      var query = $"SELECT COUNT(1) " +
                  $"FROM \"{GetTableName<Gate>()}\" " + 
                  "WHERE \"IPAddress\"=@IPAddress";
      var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
      var count = await connection.ExecuteScalarAsync<int>(query, new { IPAddress = iPAddress });
      return count > 0;
    }
}
