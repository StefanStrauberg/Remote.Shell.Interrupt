namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Helpers;

internal class SqlQueryBuilderUpdated<T> where T : BaseEntity
{
  readonly List<string> _joins;
  readonly List<string> _whereClauses;
  readonly DynamicParameters _parameters;
  readonly int? _take;
  readonly int? _skip;
  readonly List<(string Alias, Type JoinType)> _joinSelections;

  readonly SqlExpressionVisitor<T> _visitor;
  readonly ISpecification<T> _specification;

  public SqlQueryBuilderUpdated(ISpecification<T> specification)
  {
    _joins = [];
    _whereClauses = [];
    _parameters = new();
    _take = null;
    _skip = null;
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

    // Добавляем SQL условие и параметры в queryBuilder
      foreach (var param in _visitor.Parameters)
        AddFilter(whereClause, param.Value, param.Key);
    }

    if (_specification.Includes is not null)
    {
      var types = new List<Type>();

      foreach (var include in _specification.Includes)
      {
        // Разбираем тело выражения (Body)
        if (include.Body is MemberExpression memberExpression)
        {
          // Если выражение напрямую ссылается на член (например, поле или свойство)
          types.Add(memberExpression.Member.DeclaringType!);
        }
        else 
        if (include.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression member)
        {
          // Для случаев, где выражение использует преобразование (например, к object)
          types.Add(member.Member.DeclaringType!);
        }
      }
    }
  }

  SqlQueryBuilderUpdated<T> AddJoin<TJoin>(string joinClause, string alias) where TJoin : BaseEntity
  {
    _joins.Add(joinClause);
    _joinSelections.Add((alias, typeof(TJoin)));
    
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
    sql.AppendLine($"FROM \"{GetTableName.Handle<T>()}\" AS {typeof(T).Name.ToLower()}");
    
    foreach (var join in _joins)
        sql.AppendLine(join);

    if (_whereClauses.Count > 0)
        sql.AppendLine("WHERE " + string.Join(" AND ", _whereClauses));

    if (_take.HasValue)
        sql.AppendLine($"LIMIT {_take}");

    if (_skip.HasValue)
        sql.AppendLine($"OFFSET {_skip}");

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
                              .Where(p => SqlQueryBuilderUpdated<T>.IsSimpleType(p.PropertyType));
    
    // Формируем список: prefix."PropertyName"
    var prefix = typeof(T).Name.ToLower();
    var selectColumns = properties.Select(p => $"{prefix}.\"{p.Name}\"");
    return string.Join(",\n", selectColumns);
  }

  static string BuildSelectClauseJoin(string alias, Type joinType)
  {
    var properties = joinType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                             .Where(p => IsSimpleType(p.PropertyType));
    
    // Формируем список вида: alias."PropertyName"
    var selectColumns = properties.Select(p => $"{alias}.\"{p.Name}\"");
    return string.Join(",\n", selectColumns);
  }

  static bool IsSimpleType(Type type)
  {
    return type.IsPrimitive || new[] { typeof(string), typeof(decimal), typeof(DateTime), typeof(DateTimeOffset), typeof(Guid) }.Contains(type)
                            || type.IsEnum;
  }
}
