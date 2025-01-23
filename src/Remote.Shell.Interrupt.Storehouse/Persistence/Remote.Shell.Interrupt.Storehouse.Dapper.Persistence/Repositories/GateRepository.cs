namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class GateRepository(PostgreSQLDapperContext context) : GenericRepository<Gate>(context), IGateRepository
{
  async Task<bool> IGateRepository.AnyByIPAddressAsync(string iPAddress, CancellationToken cancellationToken)
  {
    var query = "SELECT COUNT(1) FROM \"Gates\" WHERE \"IPAddress\"=@IPAddress";
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var count = await connection.ExecuteScalarAsync<int>(query, new { IPAddress = iPAddress });
    return count > 0;
  }
}
