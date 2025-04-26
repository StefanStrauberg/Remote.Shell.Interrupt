namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Helpers;

internal class SqlQueryBuilderUpdated<T> where T : BaseEntity
{
  readonly List<string> _joins;
  readonly List<string> _whereClauses;
  readonly DynamicParameters _parameters;
  readonly int _take;
  readonly int _skip;
  readonly List<(string Alias, Type JoinType)> _joinSelections;

  readonly SqlExpressionVisitor<T> _visitor;
  readonly ISpecification<T> _specification;

  public SqlQueryBuilderUpdated(ISpecification<T> specification)
  {
    _joins = [];
    _whereClauses = [];
    _parameters = new();
    _joinSelections = [];
    _visitor = new();
    _specification = specification;

    if (_specification.Criteria is not null)
    {
    // Создаем посетителя для преобразования Expression в SQL
    var whereClause = _visitor.GetWhereClause(_specification.Criteria);

    // Преобразуем Expression в SQL условие
    foreach (var param in _visitor.Parameters)
      AddFilter(whereClause, param.Value, param.Key);
    }

    if (_specification.Includes is not null)
    {
        foreach (var include in _specification.Includes)
        {
            var joinInfo = SqlQueryBuilderUpdated<T>.ParseInclude(include);
            AddJoin(joinInfo);
        }
    }

    if (specification.Take > 0)
        _take = specification.Take;
    if (specification.Skip > 0)
        _skip = specification.Skip;
  }

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

    return new JoinInfo(
        JoinType: joinType,
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
    {
        return (true, type.GetGenericArguments()[0]);
    }
    
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
          
          var pkProperty = sourceType.GetProperty("Id") 
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

  SqlQueryBuilderUpdated<T> AddJoin(JoinInfo joinInfo)
  {
    var joinClause = $"LEFT JOIN \"{GetTableName.Handle(joinInfo.JoinType.Name)}\" AS {joinInfo.Alias} ON {joinInfo.Condition}";
    _joins.Add(joinClause);
    _joinSelections.Add((joinInfo.Alias, joinInfo.JoinType));
    return this;
  }

  SqlQueryBuilderUpdated<T> AddFilter(string condition, object value, string paramName)
  {
    _whereClauses.Add(condition);
    _parameters.Add(paramName, value);
    
    return this;
  }

  public (string Sql, DynamicParameters Parameters) Build()
  {
    var sql = new StringBuilder();
    
    // Формируем SELECT с полями основной сущности и join-сущностей.
    sql.Append(BuildSelectClause());

    // FROM с главной таблицей.
    sql.AppendLine($"\nFROM \"{GetTableName.Handle<T>()}\" AS {typeof(T).Name.ToLower()}");
    
    foreach (var join in _joins)
      sql.AppendLine(join);

    if (_whereClauses.Count > 0)
        sql.AppendLine("WHERE " + string.Join(" AND ", _whereClauses));

    sql.AppendLine($"LIMIT {_take}");
    sql.Append($"OFFSET {_skip}");

    return (sql.ToString(), _parameters);
  }

  string BuildSelectClause()
  {
    List<string> selectParts = [];
    selectParts.Add(BuildSelectClauseMain());
    
    foreach (var (alias, joinType) in _joinSelections)
        selectParts.Add(BuildSelectClauseJoin(alias, joinType));
    
    return "SELECT " + string.Join(",\n", selectParts);
  }

  static string BuildSelectClauseMain()
  {
    // Фильтруем свойства по типу. Можно доработать по необходимости.
    var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                              .Where(p => SqlQueryBuilderUpdated<T>.IsSimpleType(p.PropertyType))
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
    var parentId = properties.FirstOrDefault(p => p.Name == "Id" && p.DeclaringType != typeof(T));
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
  {
    return type.IsPrimitive || 
           type == typeof(string) || 
           type == typeof(decimal) || 
           type == typeof(DateTime) || 
           type == typeof(DateTimeOffset) || 
           type == typeof(Guid) || 
           type.IsEnum || 
           (type.IsGenericType && 
            type.GetGenericTypeDefinition() == typeof(Nullable<>) && 
            IsSimpleType(Nullable.GetUnderlyingType(type)!));
  }

  static string GetAlias(Type type) 
    => type.Name.ToLower();

  record JoinInfo(Type JoinType,
                  string Alias,
                  string Condition);
}
