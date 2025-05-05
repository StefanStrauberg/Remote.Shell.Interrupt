namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

internal class DbContext
{
  readonly ModelBuilder _modelBuilder = new();
  
  public string? ConnectionString { get; set; }
  public bool EnableSqlLogging { get; set; }
  public IAppLogger? Logger { get; set; }
  
  protected virtual void OnModelCreating(ModelBuilder modelBuilder) { }

  public DbContext()
  {
    OnModelCreating(_modelBuilder);
  }

  public DbSet<T> Set<T>() where T : class
    => DbSet<T>.Create(_modelBuilder, this);
  
  public DbContext EnableQueryLogging()
  {
    EnableSqlLogging = true;
    return this;
  }

  public async Task<IDbConnection> GetConnectionAsync()
  {
    var connection = new NpgsqlConnection(ConnectionString);
    await connection.OpenAsync();
    return connection;
  }

  public IDbConnection GetConnection()
  {
    var connection = new NpgsqlConnection(ConnectionString);
    connection.Open();
    return connection;
  }
}
