using System.Reflection;
using AutoMapper.Internal;

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class GenericRepository<T>(DapperContext context) : IGenericRepository<T> where T : BaseEntity
{
  protected readonly DapperContext _context = context
    ?? throw new ArgumentNullException(nameof(context));

  public async Task<bool> AnyAsync(CancellationToken cancellationToken)
  {
    string tableName = GetTableName();
    var query = $@"SELECT EXISTS ( " +
                $"SELECT 1 " +
                $"FROM \"{tableName}\")";
    using var connection = _context.CreateConnection();
    var exists = await connection.ExecuteScalarAsync<bool>(query);
    return exists;
  }

  public async Task<bool> AnyByIdAsync(Guid id,
                                       CancellationToken cancellationToken)
  {
    string tableName = GetTableName();
    var query = $@"SELECT EXISTS ( " +
                $"SELECT 1 " +
                $"FROM \"{tableName}\" " +
                "WHERE \"Id\"=@Id)";
    using var connection = _context.CreateConnection();
    var exists = await connection.ExecuteScalarAsync<bool>(query, new { Id = id });
    return exists;
  }

  public void DeleteMany(IEnumerable<T> entities)
  {
    throw new NotImplementedException();
  }

  public void DeleteOne(T entity)
  {
    string tableName = GetTableName();
    var query = $"DELETE FROM \"{tableName}\" WHERE \"Id\"=@Id";
    using var connection = _context.CreateConnection();
    connection.Execute(query, new { Id = entity.Id });
  }

  public async Task<T> FirstByIdAsync(Guid id,
                                      CancellationToken cancellationToken)
  {
    string tableName = GetTableName();
    string columns = GetColumnsAsProperties();
    var query = $"SELECT {columns} FROM \"{tableName}\" WHERE \"Id\"=@Id";
    using var connection = _context.CreateConnection();
    var company = await connection.QuerySingleAsync<T>(query, new { Id = id });
    return company;
  }

  public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
  {
    string tableName = GetTableName();
    string columns = GetColumnsAsProperties();
    var query = $"SELECT {columns} FROM \"{tableName}\"";
    using var connection = _context.CreateConnection();
    var companies = await connection.QueryAsync<T>(query);
    return companies.ToList();
  }

  public void InsertMany(IEnumerable<T> entities)
  {
    string tableName = GetTableName();
    string columns = GetColumnsAsProperties(excludeKey: true);
    string properties = GetPropertyNames(excludeKey: true);
    var query = $"INSERT INTO \"{tableName}\" ({columns}) VALUES ({properties})";
    using var connection = _context.CreateConnection();
    connection.Execute(query, entities);
  }

  public void InsertOne(T entity)
  {
    entity.CreatedAt = DateTime.Now;
    string tableName = GetTableName();
    string columns = GetColumnsAsProperties(excludeKey: true);
    string properties = GetPropertyNames(excludeKey: true);
    var query = $"INSERT INTO \"{tableName}\" ({columns}) VALUES ({properties})";
    using var connection = _context.CreateConnection();
    connection.Execute(query, entity);
  }

  public void ReplaceOne(T entity)
  {
    entity.UpdatedAt = DateTime.Now;
    string tableName = GetTableName();
    string updateProperties = GetUpdateProperties();
    var query = $"UPDATE \"{tableName}\" SET {updateProperties} WHERE \"Id\"=@Id";
    using var connection = _context.CreateConnection();
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

      sb.Append($"@{property.Name}");

      // Если это не последнее свойство, добавляем запятую
      if (i < properties.Length - 1)
        sb.Append(',');
    }

    return sb.ToString();
  }

  string GetUpdateProperties()
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

      // Используем интерполяцию строк для формирования записи
      sb.Append($"\"{property.Name}\"=@{property.Name}");

      // Если это не последнее свойство, добавляем запятую
      if (i < properties.Length - 1)
        sb.Append(", ");
    }

    return sb.ToString();
  }
}
