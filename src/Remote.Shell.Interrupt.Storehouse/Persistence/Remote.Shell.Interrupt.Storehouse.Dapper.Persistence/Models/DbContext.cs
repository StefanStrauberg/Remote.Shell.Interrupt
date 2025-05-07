namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

/// <summary>
/// Base DbContext wrapper for Dapper-powered entities
/// </summary>
internal abstract class DbContext
{
  readonly ModelBuilder _modelBuilder;

  /// <summary>
  /// Connection string for database
  /// </summary>  
  public string? ConnectionString { get; set; }

  /// <summary>
  /// Enable or disable SQL logging
  /// </summary>
  public bool EnableSqlLogging { get; set; }

  /// <summary>
  /// Application-wide logger
  /// </summary>
  public IAppLogger? Logger { get; set; }

  /// <summary>
  /// Validator factory provided by derived context via DI
  /// </summary>
  protected abstract IRelationshipValidatorFactory RelationshipValidatorFactory { get; }

  public DbContext()
  {
    _modelBuilder = new ModelBuilder(RelationshipValidatorFactory);
    OnModelCreating(_modelBuilder);
  }

  /// <summary>
  /// Configure entity mappings in derived context
  /// </summary>
  protected abstract void OnModelCreating(ModelBuilder modelBuilder);

  /// <summary>
  /// Resolve a DbSet for given entity type
  /// </summary>
  public DbSet<T> Set<T>() where T : class
    => DbSet<T>.Create(_modelBuilder, this);
  
  /// <summary>
  /// Enable fluent SQL query logging
  /// </summary>
  public DbContext EnableQueryLogging()
  {
    EnableSqlLogging = true;
    return this;
  }

  /// <summary>
  /// Get open async connection
  /// </summary>
  public async Task<IDbConnection> GetConnectionAsync()
  {
    var connection = new NpgsqlConnection(ConnectionString);
    await connection.OpenAsync();
    return connection;
  }

  /// <summary>
  /// Get open sync connection
  /// </summary>
  public IDbConnection GetConnection()
  {
    var connection = new NpgsqlConnection(ConnectionString);
    connection.Open();
    return connection;
  }
}
