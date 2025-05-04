namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

public class DbContext(string connectionString, IAppLogger logger = null!)
{
  readonly string _connectionString = connectionString;
  readonly IAppLogger _logger = logger;

  public bool EnableSqlLogging { get; set; }
  public IAppLogger Logger => _logger;
  
  protected virtual void OnModelCreating(ModelBuilder modelBuilder) { }
  
  public DbContext EnableQueryLogging(bool enable = true)
  {
    EnableSqlLogging = enable;
    return this;
  }

  public async Task<IDbConnection> GetConnectionAsync()
  {
    var connection = new NpgsqlConnection(_connectionString);
    await connection.OpenAsync();
    return connection;
  }

  public IDbConnection GetConnection()
  {
    var connection = new NpgsqlConnection(_connectionString);
    connection.Open();
    return connection;
  }
}
