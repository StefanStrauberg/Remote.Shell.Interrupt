namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class GenericRepository<T>(PostgreSQLDapperContext context) 
  : IGenericRepository<T> where T : BaseEntity
{
  protected readonly PostgreSQLDapperContext _postgreSQLDapperContext = context
    ?? throw new ArgumentNullException(nameof(context));

  void IGenericRepository<T>.DeleteMany(IEnumerable<T> entities)
  {
    _postgreSQLDapperContext.BeginTransaction();
    foreach (var entity in entities)
    {
      ((IGenericRepository<T>)this).DeleteOne(entity);
    }
  }

  async Task<T> IGenericRepository<T>.FirstByIdAsync(Guid id,
                                                     CancellationToken cancellationToken)
  {
    var query = $"SELECT {GetColumnsAsProperties()} FROM \"{GetTableName<T>()}\" WHERE \"{nameof(BaseEntity.Id)}\"=@Id";
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);

    return await connection.QuerySingleAsync<T>(query, new { Id = id });
  }

  async Task<IEnumerable<T>> IGenericRepository<T>.GetAllAsync(CancellationToken cancellationToken)
  {
    var query = $"SELECT {GetColumnsAsProperties()} FROM \"{GetTableName<T>()}\"";
    var connection = _postgreSQLDapperContext.CreateConnection();

    return await connection.QueryAsync<T>(query);
  }

  void IGenericRepository<T>.InsertMany(IEnumerable<T> entities)
  {
    _postgreSQLDapperContext.BeginTransaction();
    foreach (var entity in entities)
    {
      ((IGenericRepository<T>)this).InsertOne(entity);
    }
  }

  void IGenericRepository<T>.ReplaceMany(IEnumerable<T> entities)
  {
    _postgreSQLDapperContext.BeginTransaction();
    foreach (var entity in entities)
    {
      ((IGenericRepository<T>)this).ReplaceOne(entity);
    }
  }
}
