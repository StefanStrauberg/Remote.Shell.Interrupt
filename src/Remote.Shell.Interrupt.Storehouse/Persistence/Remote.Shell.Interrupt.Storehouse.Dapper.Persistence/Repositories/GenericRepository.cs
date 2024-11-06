namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class GenericRepository<T>(DapperContext context)
  : IGenericRepository<T> where T : BaseEntity
{
  protected readonly DapperContext _context = context;

  public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate,
                                   CancellationToken cancellationToken)
  {
    var converter = new ExpressionToStringConverter<T>();
    var lookingFor = converter.Convert(predicate);

    var query = $"SELECT COUNT(1) FROM {typeof(T).Name}s WHERE {lookingFor}";

    using var connection = _context.CreateConnection();
    var count = await connection.ExecuteScalarAsync<int>(query, cancellationToken);

    return count > 0;
  }

  public void DeleteMany(IEnumerable<T> entities)
  {
    throw new NotImplementedException();
  }

  public void DeleteOne(T entity)
  {
    var query = $"DELETE FROM {typeof(T).Name}s WHERE Id = @Id";

    using var connection = _context.CreateConnection();

    connection.Execute(query, entity.Id);
  }

  public async Task<T> FirstAsync(Expression<Func<T, bool>> predicate,
                                  CancellationToken cancellationToken)
  {
    var converter = new ExpressionToStringConverter<T>();
    var lookingFor = converter.Convert(predicate);

    var query = $"SELECT * FROM {typeof(T).Name}s WHERE {lookingFor} LIMIT 1";

    using var connection = _context.CreateConnection();

    var result = await connection.QueryFirstOrDefaultAsync<T>(query, cancellationToken);

    return result ?? throw new Exception($"During sql \"{query}\" execution was receive null value.");
  }

  public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
  {
    var query = $"SELECT * FROM \"{typeof(T).Name}s\"";

    using var connection = _context.CreateConnection();

    var result = await connection.QueryAsync<T>(query, cancellationToken);

    return result.ToList();
  }

  public void InsertMany(IEnumerable<T> entities)
  {
    throw new NotImplementedException();
  }

  public void InsertOne(T entity)
  {
    string tableName = $"{typeof(T).Name}s";
    string columns = GenericRepository<T>.GetColumns(excludeKey: true);
    string properties = GetPropertyNames(excludeKey: true);
    string query = $"INSERT INTO \"{tableName}\" ({columns}) VALUES ({properties})";

    using var connection = _context.CreateConnection();

    connection.Execute(query, entity);
  }

  public void ReplaceOne(T entity)
  {
    throw new NotImplementedException();
  }

  private static string GetColumns(bool excludeKey = false)
  {
    var type = typeof(T);
    var properties = type.GetProperties();
    var columns = string.Join(", ", type.GetProperties()
                                        .Where(p => !excludeKey || !p.IsDefined(typeof(KeyAttribute)))
                                        .Select(p =>
                                        {
                                          var columnAttr = p.GetCustomAttribute<ColumnAttribute>();
                                          return columnAttr != null ? columnAttr.Name : p.Name;
                                        }));

    return columns;
  }

  protected string GetPropertyNames(bool excludeKey = false)
  {
    var properties = typeof(T).GetProperties()
                              .Where(p => !excludeKey || p.GetCustomAttribute<KeyAttribute>() == null);

    var values = string.Join(", ", properties.Select(p =>
                                                     {
                                                       return $"@{p.Name}";
                                                     }));

    return values;
  }
}
