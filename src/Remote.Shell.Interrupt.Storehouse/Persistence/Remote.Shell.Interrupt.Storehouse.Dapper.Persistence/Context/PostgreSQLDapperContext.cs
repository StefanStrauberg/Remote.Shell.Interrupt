namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Context;

internal class PostgreSQLDapperContext(IConfiguration configuration) : IDisposable
{
  readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!
    ?? throw new ArgumentException(nameof(configuration));
  NpgsqlConnection? _dbConnection;
  NpgsqlTransaction? _transaction;
  bool _isTransactionActive = false; // Поле для отслеживания состояния транзакции

  public NpgsqlConnection CreateConnection()
  {
    if (_dbConnection is null)
    {
      _dbConnection = new NpgsqlConnection(_connectionString);
      _dbConnection.Open();
      return _dbConnection;
    }
    else if (_dbConnection.State is not ConnectionState.Open)
    {
      _dbConnection.Open();
      return _dbConnection;
    }
    return _dbConnection;
  }

  public async Task<NpgsqlConnection> CreateConnectionAsync(CancellationToken cancellationToken)
  {
    if (_dbConnection is null)
    {
      _dbConnection = new NpgsqlConnection(_connectionString);
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

  public void BeginTransaction()
  {
    if (_isTransactionActive)
      return;

    if (_dbConnection is null)
    {
      _dbConnection = new NpgsqlConnection(_connectionString);
      _dbConnection.Open();
      _transaction = _dbConnection.BeginTransaction();
      _isTransactionActive = true;
      return;
    }
    else if (_dbConnection.State is not ConnectionState.Open)
    {
      _dbConnection.Open();
      _transaction = _dbConnection.BeginTransaction();
      _isTransactionActive = true;
      return;
    }

    _transaction = _dbConnection.BeginTransaction();
    _isTransactionActive = true;
  }

  public void CompleteTransaction()
  {
    if (!_isTransactionActive)
      throw new InvalidOperationException("No active transaction to complete.");

    try
    {
      // Завершение транзакции
      _transaction!.Commit();
      _isTransactionActive = false; // Сбрасываем флаг после завершения транзакции
    }
    catch (Exception ex)
    {
      // В случае ошибки откатываем транзакцию
      _transaction!.Rollback();
      _isTransactionActive = false; // Сбрасываем флаг при ошибке
      throw new Exception(ex.ToString());
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
      // Освобождаем управляемые ресурсы
      if (_dbConnection is not null)
      {
        _transaction?.Dispose();
        _transaction = null; // Убираем ссылку на транзакцию
        _isTransactionActive = false; // Сбрасываем флаг при завершении

        if (_dbConnection.State == ConnectionState.Open)
          _dbConnection.Close(); // Закрываем соединение, если оно открыто

        _dbConnection.Dispose(); // Освобождаем ресурсы соединения
        _dbConnection = null; // Убираем ссылку на соединение
      }
    }
  }
}
