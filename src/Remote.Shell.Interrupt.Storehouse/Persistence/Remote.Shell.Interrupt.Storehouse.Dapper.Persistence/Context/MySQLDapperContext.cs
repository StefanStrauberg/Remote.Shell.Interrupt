namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Context;

internal class MySQLDapperContext(IConfiguration configuration) : IDisposable
{
  readonly string _connectionString = configuration.GetConnectionString("DefaultConnection2")
    ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection2' in configuration.");
  readonly SemaphoreSlim _connectionLock = new(1, 1);
  MySqlConnection? _dbConnection;

  public async Task<MySqlConnection> CreateConnectionAsync(CancellationToken cancellationToken)
  {
    if (_dbConnection is not null && _dbConnection.State is ConnectionState.Open)
      return _dbConnection;

    // Guard creation/reopen: this context is shared by every repository resolved within
    // the same scope, so concurrent calls (e.g. via Task.WhenAll) could otherwise race
    // into opening the same non-thread-safe MySqlConnection instance concurrently.
    await _connectionLock.WaitAsync(cancellationToken);
    try
    {
      if (_dbConnection is null)
        _dbConnection = new MySqlConnection(_connectionString);

      if (_dbConnection.State is not ConnectionState.Open)
        await _dbConnection.OpenAsync(cancellationToken);

      return _dbConnection;
    }
    finally
    {
      _connectionLock.Release();
    }
  }

  void IDisposable.Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (disposing)
    {
      // Release managed resources
      if (_dbConnection is not null)
      {
        if (_dbConnection.State == ConnectionState.Open)
          _dbConnection.Close(); // Close the connection if it is open

        _dbConnection.Dispose(); // Release the connection resources
        _dbConnection = null; // Drop the connection reference
      }

      _connectionLock.Dispose();
    }
  }
}
