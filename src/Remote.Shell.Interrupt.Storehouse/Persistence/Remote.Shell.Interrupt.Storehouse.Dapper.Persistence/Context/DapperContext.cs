namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Context;

internal class DapperContext(IConfiguration configuration) : IDisposable
{
  readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!
    ?? throw new ArgumentException(nameof(configuration));
  IDbConnection? _connection;


  public IDbConnection CreateConnection()
  {
    // Создание нового соединения при каждом вызове
    var connection = new NpgsqlConnection(_connectionString);
    connection.Open(); // Открываем соединение
    return connection; // Возвращаем новое соединение
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (disposing)
    {
      // Освобождаем управляемые ресурсы
      if (_connection != null)
      {
        if (_connection.State == ConnectionState.Open)
        {
          _connection.Close(); // Закрываем соединение, если оно открыто
        }
        _connection.Dispose(); // Освобождаем ресурсы соединения
        _connection = null; // Убираем ссылку на соединение
      }
    }
  }
}
