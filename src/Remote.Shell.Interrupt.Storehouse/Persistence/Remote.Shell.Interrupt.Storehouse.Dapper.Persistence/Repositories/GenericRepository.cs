namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class GenericRepository<T>(PostgreSQLDapperContext context) : IGenericRepository<T> where T : BaseEntity
{
  protected readonly PostgreSQLDapperContext _postgreSQLDapperContext = context
    ?? throw new ArgumentNullException(nameof(context));

  async Task<bool> IGenericRepository<T>.AnyAsync(CancellationToken cancellationToken)
  {
    var query = $"SELECT COUNT(1) FROM \"{GetTableName<T>()}\"";
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var count = await connection.ExecuteScalarAsync<int>(query);
    return count > 0;
  }

  async Task<bool> IGenericRepository<T>.AnyByIdAsync(Guid id,
                                       CancellationToken cancellationToken)
  {
    var query = $"SELECT COUNT(1) FROM \"{GetTableName<T>()}\" WHERE \"Id\"=@Id";
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var count = await connection.ExecuteScalarAsync<int>(query, new { Id = id });
    return count > 0;
  }

  void IGenericRepository<T>.DeleteMany(IEnumerable<T> entities)
  {
    _postgreSQLDapperContext.BeginTransaction();
    foreach (var entity in entities)
    {
      ((IGenericRepository<T>)this).DeleteOne(entity);
    }
  }

  void IGenericRepository<T>.DeleteOne(T entity)
  {
    _postgreSQLDapperContext.BeginTransaction();
    var query = $"DELETE FROM \"{GetTableName<T>()}\" WHERE \"Id\"=@Id";
    var connection = _postgreSQLDapperContext.CreateConnection();
    connection.Execute(query, new { Id = entity.Id });
  }

  async Task<T> IGenericRepository<T>.FirstByIdAsync(Guid id,
                                                     CancellationToken cancellationToken)
  {
    string columns = GetColumnsAsProperties();
    var query = $"SELECT {columns} FROM \"{GetTableName<T>()}\" WHERE \"Id\"=@Id";
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    return await connection.QuerySingleAsync<T>(query, new { Id = id });
  }

  async Task<IEnumerable<T>> IGenericRepository<T>.GetAllAsync(CancellationToken cancellationToken)
  {
    string columns = GetColumnsAsProperties();
    var query = $"SELECT {columns} FROM \"{GetTableName<T>()}\"";
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

  void IGenericRepository<T>.InsertOne(T entity)
  {
    _postgreSQLDapperContext.BeginTransaction();
    string columns = GetColumnsAsProperties(excludeKey: true);
    string properties = GetPropertyNames(excludeKey: true);
    var query = $"INSERT INTO \"{GetTableName<T>()}\" ({columns}) VALUES ({properties}) RETURNING \"Id\"";
    var connection = _postgreSQLDapperContext.CreateConnection();
    var entityId = connection.ExecuteScalar<Guid>(query, entity);
    entity.Id = entityId;
  }

  void IGenericRepository<T>.ReplaceOne(T entity)
  {
    _postgreSQLDapperContext.BeginTransaction();
    string updateProperties = GetUpdateProperties(excludeKey: true);
    var query = $"UPDATE \"{GetTableName<T>()}\" SET {updateProperties} WHERE \"Id\"=@Id";
    var connection = _postgreSQLDapperContext.CreateConnection();
    connection.Execute(query, entity);
  }

  void IGenericRepository<T>.ReplaceMany(IEnumerable<T> entities)
  {
    _postgreSQLDapperContext.BeginTransaction();
    foreach (var entity in entities)
    {
      ((IGenericRepository<T>)this).ReplaceOne(entity);
    }
  }

  static protected string GetTableName<K>()
  {
    StringBuilder sb = new();
    var name = typeof(K).Name;

    if (name[^1] == 'y')
    {
      sb.Append(name, 0, name.Length - 1); // Добавляем все символы, кроме последнего
      sb.Append("ies");
    }
    else
    {
      sb.Append(name);
      sb.Append('s');
    }

    return sb.ToString();
  }

  static protected string GetColumnsAsProperties(bool excludeKey = false)
  {
    StringBuilder sb = new();
    Type type = typeof(T);

    // Получаем все свойства типа T, включая унаследованные
    PropertyInfo[] properties = type.GetProperties(BindingFlags.Public |
                                                   BindingFlags.NonPublic |
                                                   BindingFlags.Instance |
                                                   BindingFlags.Static |
                                                   BindingFlags.FlattenHierarchy);

    // Извлекаем имена свойств
    for (int i = 0; i < properties.Length; i++)
    {
      var property = properties[i];

      // Проверяем, является ли свойство классом или коллекцией
      bool isClass = property.PropertyType.IsClass && property.PropertyType != typeof(string);
      bool isCollection = typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string);

      // Если свойство является классом или коллекцией, пропускаем его
      if (isClass || isCollection)
        continue;

      // Проверяем, является ли свойство Id
      bool isGuuid = property.Name.Equals("Id");

      // Если свойство является GUUID и excludeKey = true, пропускаем его
      if (excludeKey && isGuuid)
        continue;

      // Если свойство CreatedAt, пропускаем его
      if (property.Name.Equals("CreatedAt"))
        continue;

      // Если свойство UpdatedAt, пропускаем его
      if (property.Name.Equals("UpdatedAt"))
        continue;

      sb.Append($"\"{property.Name}\"");

      sb.Append(',');
    }

    if (sb[^1] == ',')
      sb.Length--;

    return sb.ToString();
  }

  static protected string GetPropertyNames(bool excludeKey = false)
  {
    StringBuilder sb = new();
    Type type = typeof(T);

    // Получаем все свойства типа T, включая унаследованные
    PropertyInfo[] properties = type.GetProperties(BindingFlags.Public |
                                                   BindingFlags.NonPublic |
                                                   BindingFlags.Instance |
                                                   BindingFlags.Static |
                                                   BindingFlags.FlattenHierarchy);

    // Извлекаем имена свойств
    for (int i = 0; i < properties.Length; i++)
    {
      var property = properties[i];

      // Проверяем, является ли свойство классом или коллекцией
      bool isClass = property.PropertyType.IsClass && property.PropertyType != typeof(string);
      bool isCollection = typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string);

      // Если свойство является классом или коллекцией, пропускаем его
      if (isClass || isCollection)
        continue;

      // Проверяем, является ли свойство Id
      bool isGuuid = property.Name.Equals("Id");

      // Если свойство является GUUID и excludeKey = true, пропускаем его
      if (excludeKey && isGuuid)
        continue;

      // Если свойство CreatedAt, пропускаем его
      if (property.Name.Equals("CreatedAt"))
        continue;

      // Если свойство UpdatedAt, пропускаем его
      if (property.Name.Equals("UpdatedAt"))
        continue;

      sb.Append($"@{property.Name}");

      sb.Append(',');
    }

    if (sb[^1] == ',')
      sb.Length--;

    return sb.ToString();
  }

  static string GetUpdateProperties(bool excludeKey = false)
  {
    StringBuilder sb = new();
    Type type = typeof(T);

    // Получаем все свойства типа T, включая унаследованные
    PropertyInfo[] properties = type.GetProperties(BindingFlags.Public |
                                                   BindingFlags.NonPublic |
                                                   BindingFlags.Instance |
                                                   BindingFlags.Static |
                                                   BindingFlags.FlattenHierarchy);

    // Извлекаем имена свойств
    for (int i = 0; i < properties.Length; i++)
    {
      var property = properties[i];

      // Проверяем, является ли свойство классом или коллекцией
      bool isClass = property.PropertyType.IsClass && property.PropertyType != typeof(string);
      bool isCollection = typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string);

      // Если свойство является классом или коллекцией, пропускаем его
      if (isClass || isCollection)
        continue;

      // Проверяем, является ли свойство Id
      bool isGuuid = property.Name.Equals("Id");

      // Если свойство является GUUID и excludeKey = true, пропускаем его
      if (excludeKey && isGuuid)
        continue;

      // Если свойство CreatedAt, пропускаем его
      if (property.Name.Equals("CreatedAt"))
        continue;

      // Если свойство UpdatedAt, пропускаем его
      if (property.Name.Equals("UpdatedAt"))
        continue;

      // Используем интерполяцию строк для формирования записи
      sb.Append($"\"{property.Name}\"=@{property.Name}");

      sb.Append(", ");
    }

    if (sb[^1] == ' ')
      sb.Length -= 2;

    return sb.ToString();
  }

  async Task<int> IGenericRepository<T>.GetCountAsync(CancellationToken cancellationToken)
  {
    var query = $"SELECT COUNT(\"Id\") FROM \"{GetTableName<T>()}\"";
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var count = await connection.ExecuteScalarAsync<int>(query);
    return count;
  }
}
