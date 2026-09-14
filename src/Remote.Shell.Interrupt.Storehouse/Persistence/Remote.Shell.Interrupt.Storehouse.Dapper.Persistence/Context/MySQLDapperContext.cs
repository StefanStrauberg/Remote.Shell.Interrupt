namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Context;

/// <summary>
/// See <see cref="IMySqlConnectionFactory"/> for why this database is accessed via raw
/// Dapper SQL rather than EF Core, and why write access is actively rejected rather
/// than merely "not used".
/// </summary>
internal class MySQLDapperContext(IConfiguration configuration) : IMySqlConnectionFactory, IDisposable
{
  readonly string _connectionString = configuration.GetConnectionString("DefaultConnection2")
    ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection2' in configuration.");
  readonly SemaphoreSlim _connectionLock = new(1, 1);
  MySqlConnection? _dbConnection;

  public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken)
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
      {
        await _dbConnection.OpenAsync(cancellationToken);
        await EnforceReadOnlySessionAsync(_dbConnection, cancellationToken);
      }

      return _dbConnection;
    }
    finally
    {
      _connectionLock.Release();
    }
  }

  /// <summary>
  /// Defense in depth: this application is only permitted to read from the remote
  /// billing database, regardless of what the connecting account's own grants allow.
  /// Making the session read-only at the MySQL engine level means an accidental future
  /// INSERT/UPDATE/DELETE added to a RemBillRep repository fails loudly with a MySQL
  /// error instead of silently succeeding against a database this application does not
  /// own and cannot safely repair if corrupted.
  /// </summary>
  static async Task EnforceReadOnlySessionAsync(MySqlConnection connection, CancellationToken cancellationToken)
  {
    using var command = connection.CreateCommand();
    command.CommandText = "SET SESSION TRANSACTION READ ONLY";
    await command.ExecuteNonQueryAsync(cancellationToken);
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
