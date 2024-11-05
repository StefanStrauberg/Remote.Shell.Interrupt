namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Context;

public class DapperContext
{
  private readonly IConfiguration _configuration;
  private readonly string _connectionString;
  public DapperContext(IConfiguration configuration)
  {
    _configuration = configuration;
    _connectionString = _configuration.GetConnectionString("SqlConnection")!;
  }
  public IDbConnection CreateConnection()
      => new NpgsqlConnection(_connectionString);
}
