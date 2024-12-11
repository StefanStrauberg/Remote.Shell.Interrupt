namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Context;

internal class MySQLDapperContext(IConfiguration configuration) : IDisposable
{
  readonly string _connectionString = configuration.GetConnectionString("DefaultConnection2")!
    ?? throw new ArgumentException(nameof(configuration));
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
      _dbConnection.Open();
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
      // Освобождаем управляемые ресурсы
      if (_dbConnection is not null)
      {
        if (_dbConnection.State == ConnectionState.Open)
          _dbConnection.Close(); // Закрываем соединение, если оно открыто

        _dbConnection.Dispose(); // Освобождаем ресурсы соединения
        _dbConnection = null; // Убираем ссылку на соединение
      }
    }
  }
}
