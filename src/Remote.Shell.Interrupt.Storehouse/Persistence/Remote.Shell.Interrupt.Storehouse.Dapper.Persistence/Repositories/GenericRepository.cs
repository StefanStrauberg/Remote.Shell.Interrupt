namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class GenericRepository<T>(DapperContext context) : IGenericRepository<T> where T : BaseEntity
{
  protected readonly DapperContext _context = context
    ?? throw new ArgumentNullException(nameof(context));

  public async Task<bool> AnyAsync(CancellationToken cancellationToken)
  {
    string tableName = GetTableName();
    var query = $"SELECT COUNT(1) FROM \"{tableName}\"";
    var connection = await _context.CreateConnectionAsync(cancellationToken);
    var count = await connection.ExecuteScalarAsync<int>(query);
    return count > 0;
  }

  public async Task<bool> AnyByIdAsync(Guid id,
                                       CancellationToken cancellationToken)
  {
    string tableName = GetTableName();
    var query = $"SELECT COUNT(1) FROM \"{tableName}\" WHERE \"Id\"=@Id";
    var connection = await _context.CreateConnectionAsync(cancellationToken);
    var count = await connection.ExecuteScalarAsync<int>(query, new { Id = id });
    return count > 0;
  }

  public void DeleteMany(IEnumerable<T> entities)
  {
    _context.BeginTransaction();
    foreach (var entity in entities)
    {
      DeleteOne(entity);
    }
  }

  public void DeleteOne(T entity)
  {
    _context.BeginTransaction();
    string tableName = GetTableName();
    var query = $"DELETE FROM \"{tableName}\" WHERE \"Id\"=@Id";
    var connection = _context.CreateConnection();
    connection.Execute(query, new { Id = entity.Id });
  }

  public async Task<T> FirstByIdAsync(Guid id,
                                      CancellationToken cancellationToken)
  {
    string tableName = GetTableName();
    string columns = GetColumnsAsProperties();
    var query = $"SELECT {columns} FROM \"{tableName}\" WHERE \"Id\"=@Id";
    var connection = await _context.CreateConnectionAsync(cancellationToken);
    return await connection.QuerySingleAsync<T>(query, new { Id = id });
  }

  public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
  {
    string tableName = GetTableName();
    string columns = GetColumnsAsProperties();
    var query = $"SELECT {columns} FROM \"{tableName}\"";
    var connection = _context.CreateConnection();
    return await connection.QueryAsync<T>(query);
  }

  public void InsertMany(IEnumerable<T> entities)
  {
    _context.BeginTransaction();
    foreach (var entity in entities)
    {
      InsertOne(entity);
    }
  }

  public void InsertOne(T entity)
  {
    _context.BeginTransaction();
    string tableName = GetTableName();
    string columns = GetColumnsAsProperties(excludeKey: true);
    string properties = GetPropertyNames(excludeKey: true);
    var query = $"INSERT INTO \"{tableName}\" ({columns}) VALUES ({properties}) RETURNING \"Id\"";
    var connection = _context.CreateConnection();
    var entityId = connection.ExecuteScalar<Guid>(query, entity);
    entity.Id = entityId;
  }

  public void ReplaceOne(T entity)
  {
    _context.BeginTransaction();
    entity.UpdatedAt = DateTime.Now;
    string tableName = GetTableName();
    string updateProperties = GetUpdateProperties(excludeKey: true);
    var query = $"UPDATE \"{tableName}\" SET {updateProperties} WHERE \"Id\"=@Id";
    var connection = _context.CreateConnection();
    connection.Execute(query, entity);
  }

  protected string GetTableName()
  {
    StringBuilder sb = new();
    var name = typeof(T).Name;

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

  protected string GetColumnsAsProperties(bool excludeKey = false)
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

      sb.Append($"\"{property.Name}\"");

      // Если это не последнее свойство, добавляем запятую
      if (i < properties.Length - 1)
      {
        sb.Append(',');
      }
    }

    return sb.ToString();
  }

  protected string GetPropertyNames(bool excludeKey = false)
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

      sb.Append($"@{property.Name}");

      // Если это не последнее свойство, добавляем запятую
      if (i < properties.Length - 1)
        sb.Append(',');
    }

    return sb.ToString();
  }

  string GetUpdateProperties(bool excludeKey = false)
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

      // Используем интерполяцию строк для формирования записи
      sb.Append($"\"{property.Name}\"=@{property.Name}");

      // Если это не последнее свойство, добавляем запятую
      if (i < properties.Length - 1)
        sb.Append(", ");
    }

    return sb.ToString();
  }
}
