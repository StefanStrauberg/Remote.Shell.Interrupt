namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Context;

internal class MySQLDapperContext(IConfiguration configuration) : IDisposable
{
  readonly string _connectionString = configuration.GetConnectionString("DefaultConnection2")
    ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection2' in configuration.");
  MySqlConnection? _dbConnection;

  public async Task<MySqlConnection> CreateConnectionAsync(CancellationToken cancellationToken)
  {
    if (_dbConnection is null)
    {
      _dbConnection = new MySqlConnection(_connectionString);
      await _dbConnection.OpenAsync(cancellationToken);
      return _dbConnection;
    }
    else if (_dbConnection.State is not ConnectionState.Open)
    {
      await _dbConnection.OpenAsync(cancellationToken);
      return _dbConnection;
    }
    return _dbConnection;
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
    }
  }
}
