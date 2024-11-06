namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Context;

internal class DapperContext : IDisposable
{
  readonly IConfiguration _configuration;
  readonly string _connectionString;
  NpgsqlConnection _connection = null!;
  bool _disposed = false;

  public DapperContext(IConfiguration configuration)
  {
    _configuration = configuration;
    _connectionString = _configuration.GetConnectionString("DefaultConnection")!;
  }
  public NpgsqlConnection CreateConnection()
  {
    _connection ??= new NpgsqlConnection(_connectionString);
    return _connection;
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!_disposed)
    {
      if (disposing)
        _connection?.Dispose();

      _disposed = true;
    }
  }

  ~DapperContext()
  {
    Dispose(false);
  }
}
