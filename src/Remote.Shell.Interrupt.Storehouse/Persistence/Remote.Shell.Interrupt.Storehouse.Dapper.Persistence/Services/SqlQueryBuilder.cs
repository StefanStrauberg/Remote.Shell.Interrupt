namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Services;

internal class SqlQueryBuilder<T>(ISpecification<T> specification) where T : BaseEntity
{
  readonly List<string> _joins = [];
  readonly List<string> _whereClauses = [];
  int _take;
  int _skip;
  readonly List<(string Alias, Type JoinType)> _joinSelections = [];

  readonly SqlExpressionVisitor<T> _visitor = new();
  readonly ISpecification<T> _specification = specification;

  public SqlQueryBuilder() : this(null!)
  { }

  static JoinInfo ParseInclude(Expression<Func<T, object>> include)
  {
    var memberExpression = include.Body switch
    {
      MemberExpression m => m,
      UnaryExpression u when u.Operand is MemberExpression m => m,
      _ => throw new ArgumentException("Invalid include expression")
    };

    var propInfo = memberExpression.Member as PropertyInfo;
    var navigationProperty = propInfo!;
    
    var (isCollection, joinType) = GetNavigationPropertyType(navigationProperty);
    var (fkProperty, pkProperty) = ResolveKeys(typeof(T), navigationProperty, joinType, isCollection);

    return new JoinInfo(JoinType: joinType,
                        Alias: joinType.Name.ToLower(),
                        Condition: $"{GetAlias(joinType)}.\"{pkProperty.Name}\" = " +
                                   $"{GetAlias(typeof(T))}.\"{fkProperty.Name}\""
    );
  }

  static (bool isCollection, Type joinType) GetNavigationPropertyType(PropertyInfo property)
  {
    var type = property.PropertyType;
    
    if (type.IsGenericType && 
        type.GetGenericTypeDefinition() == typeof(List<>))
      return (true, type.GetGenericArguments()[0]);
    
    return (false, type);
  }

  static (PropertyInfo fkProperty, PropertyInfo pkProperty) ResolveKeys(Type sourceType,
                                                                        PropertyInfo navigationProperty,
                                                                        Type joinType,
                                                                        bool isCollection)
  {
    if (isCollection)
    {
      // Для коллекций FK находится в joinType
      var fkName = $"Id{sourceType.Name}";
      var fkProperty = joinType.GetProperty(fkName) 
          ?? throw new KeyNotFoundException($"Foreign key {fkName} not found in {joinType.Name}");
      
      var pkProperty = sourceType.GetProperty($"Id{sourceType.Name}") 
          ?? throw new KeyNotFoundException($"Primary key Id not found in {sourceType.Name}");

      return (fkProperty, pkProperty);
    }
    else
    {
      // Для одиночных навигаций FK находится в sourceType
      var fkName = $"Id_{navigationProperty.Name}";
      var fkProperty = sourceType.GetProperty(fkName) 
          ?? throw new KeyNotFoundException($"Foreign key {fkName} not found in {sourceType.Name}");
      
      var pkName = $"Id{joinType.Name}";
      var pkProperty = joinType.GetProperty(pkName) 
          ?? throw new KeyNotFoundException($"Primary key {pkName} not found in {joinType.Name}");

      return (fkProperty, pkProperty);
    }
  }

  SqlQueryBuilder<T> AddJoin(JoinInfo joinInfo)
  {
    var joinClause = $"LEFT JOIN \"{GetTableName.Handle(joinInfo.JoinType.Name)}\" AS {joinInfo.Alias} ON {joinInfo.Condition}";
    _joins.Add(joinClause);
    _joinSelections.Add((joinInfo.Alias, joinInfo.JoinType));
    return this;
  }

  SqlQueryBuilder<T> AddFilter(string condition)
  {
    _whereClauses.Add(condition);   
    return this;
  }

  public string Build()
  {
    if (_specification is not null)
      Prepare();

    var sql = new StringBuilder();

    // Формируем SELECT с полями основной сущности и join-сущностей.
    sql.Append(BuildSelectClause());

    // FROM с главной таблицей.
    sql.AppendLine($"\nFROM \"{GetTableName.Handle<T>()}\" AS {typeof(T).Name.ToLower()}");

    foreach (var join in _joins)
      sql.AppendLine(join);

    if (_whereClauses.Count > 0)
      sql.AppendLine("WHERE " + string.Join(" AND ", _whereClauses));

    if (IsPaginated())
    {
      sql.AppendLine($"LIMIT {_take}");
      sql.Append($"OFFSET {_skip}");
    }

    return sql.ToString();
  }

  void Prepare()
  {
    if (_specification.Criterias is not null)
    {
      // Создаем посетителя для преобразования Expression в SQL
      var whereClause = _visitor.GetWhereClause(_specification.Criterias);

      AddFilter(whereClause);
    }

    if (_specification.Includes is not null)
    {
      foreach (var include in _specification.Includes)
      {
        var joinInfo = SqlQueryBuilder<T>.ParseInclude(include);
        AddJoin(joinInfo);
      }
    }

    if (_specification.Take > 0)
      _take = _specification.Take;
    if (_specification.Skip > 0)
      _skip = _specification.Skip;
  }

  public string BuildCount()
  {
    if (_specification is not null)
      Prepare();

    var sql = new StringBuilder();

    // Формируем SELECT с полями основной сущности и join-сущностей.
    sql.AppendLine(SqlQueryBuilder<T>.BuildCountClause());

    // FROM с главной таблицей.
    sql.AppendLine($"FROM \"{GetTableName.Handle<T>()}\" AS {typeof(T).Name.ToLower()}");

    foreach (var join in _joins)
      sql.AppendLine(join);

    if (_whereClauses.Count > 0)
      sql.AppendLine("WHERE " + string.Join(" AND ", _whereClauses));

    if (IsPaginated())
    {
      sql.AppendLine($"LIMIT {_take}");
      sql.Append($"OFFSET {_skip}");
    }

    return sql.ToString();
  }

  public string BuildDelete(Guid id)
  {
    if (_specification is not null)
      Prepare();

    var sql = new StringBuilder();

    // Формируем SELECT с полями основной сущности и join-сущностей.
    sql.AppendLine(SqlQueryBuilder<T>.BuildDeleteClause());

    // FROM с главной таблицей.
    sql.AppendLine($"FROM \"{GetTableName.Handle<T>()}\" AS {typeof(T).Name.ToLower()}");

    sql.AppendLine($"WHERE {typeof(T).Name.ToLower()}.\"{nameof(BaseEntity.Id)}\" = '{id.ToString()}'");

    return sql.ToString();
  }

  public string BuildDeleteMany(IEnumerable<Guid> ids)
  {
    if (_specification is not null)
      Prepare();

    var sql = new StringBuilder();

    // Формируем SELECT с полями основной сущности и join-сущностей.
    sql.AppendLine(SqlQueryBuilder<T>.BuildDeleteClause());

    // FROM с главной таблицей.
    sql.AppendLine($"FROM \"{GetTableName.Handle<T>()}\" AS {typeof(T).Name.ToLower()}");

    sql.AppendLine($"WHERE {typeof(T).Name.ToLower()}.\"{nameof(BaseEntity.Id)}\" IN ({string.Join(", ", ids.Select(g => $"'{g}'"))})");

    return sql.ToString();
  }

  bool IsPaginated()
    => _take > 0 && _skip >= 0;

  string BuildSelectClause()
  {
    List<string> selectParts = [];
    selectParts.Add(BuildSelectClauseMain());
    
    foreach (var (alias, joinType) in _joinSelections)
      selectParts.Add(BuildSelectClauseJoin(alias, joinType));
    
    return "SELECT " + string.Join(",\n", selectParts);
  }

  static string BuildCountClause()
    => "SELECT COUNT(*)";

  static string BuildDeleteClause()
    => "DELETE ";

  static string BuildSelectClauseMain()
  {
    // Фильтруем свойства по типу. Можно доработать по необходимости.
    var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                              .Where(p => SqlQueryBuilder<T>.IsSimpleType(p.PropertyType))
                              .ToList();

    // Если свойство "Id", определённое в базовом классе, присутствует, переместим его в начало
    var parentId = properties.FirstOrDefault(p => p.Name == "Id" && p.DeclaringType != typeof(T));
    if (parentId != null)
    {
        properties.Remove(parentId);
        properties.Insert(0, parentId);
    }
    
    // Формируем список: prefix."PropertyName"
    var prefix = typeof(T).Name.ToLower();
    var selectColumns = properties.Select(p => $"{prefix}.\"{p.Name}\"");
    return string.Join(",\n", selectColumns);
  }

  static string BuildSelectClauseJoin(string alias, Type joinType)
  {
    var properties = joinType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                             .Where(p => IsSimpleType(p.PropertyType))
                             .ToList();

    // Если свойство "Id", определённое в базовом классе, присутствует, переместим его в начало
    var parentId = properties.FirstOrDefault(p => p.Name == nameof(BaseEntity.Id) && 
                                             p.DeclaringType != typeof(T));
    if (parentId != null)
    {
      properties.Remove(parentId);
      properties.Insert(0, parentId);
    }
    
    // Формируем список вида: alias."PropertyName"
    var selectColumns = properties.Select(p => $"{alias}.\"{p.Name}\"");
    return string.Join(",\n", selectColumns);
  }

  static bool IsSimpleType(Type type)
    => type.IsPrimitive || 
       type == typeof(string) || 
       type == typeof(decimal) || 
       type == typeof(DateTime) || 
       type == typeof(DateTimeOffset) || 
       type == typeof(Guid) || 
       type.IsEnum || 
       (type.IsGenericType && 
        type.GetGenericTypeDefinition() == typeof(Nullable<>) && 
        IsSimpleType(Nullable.GetUnderlyingType(type)!));

  static string GetAlias(Type type) 
    => type.Name.ToLower();

  record JoinInfo(Type JoinType,
                  string Alias,
                  string Condition);
}
