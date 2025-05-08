namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

internal class SQLQueryBuilder<TEntity>(ModelBuilder modelBuilder,
                                        List<string> includes,
                                        string whereClause,
                                        int? skip,
                                        int? take)
  where TEntity : class
{
  readonly ModelBuilder _modelBuilder = modelBuilder;
  readonly List<string> _includes = includes;
  readonly string _whereClause = whereClause;
  readonly int? _skip = skip;
  readonly int? _take = take;
  public string CustomSelect { get; set; } = null!;
  public string FromClause { get; set; } = null!;

  public string BuildQuery()
  {
    var sb = new StringBuilder();
    var entityConfig = _modelBuilder.Configurations[typeof(TEntity)];
    var mainTable = entityConfig.TableName;
    var mainTableAlias = mainTable.ToLower();
    
    // Формируем SELECT-часть:
    // Если включения заданы или задан CustomSelect, то полагаем, что нам нужен алиас.
    var select = _includes.Count > 0 
                   ? BuildSelectClause(entityConfig, mainTableAlias)
                   : (string.IsNullOrEmpty(CustomSelect) ? "*" : CustomSelect);
    var from = string.IsNullOrEmpty(FromClause) ? mainTable : FromClause;
    
    if (select == "*")
      sb.Append($"SELECT {select} FROM \"{from}\"");
    else
      sb.Append($"SELECT {select} FROM \"{from}\" {mainTableAlias}");

    // Добавляем JOINы для каждого include.
    foreach (var include in _includes)
      sb.Append(BuildJoinClause(include, mainTableAlias, entityConfig));

    if (!string.IsNullOrEmpty(_whereClause))
      sb.Append($" WHERE {_whereClause}");

    if (_take.HasValue && _skip.HasValue)
      sb.Append($" LIMIT {_take.Value} OFFSET {_skip.Value}");

    return sb.ToString();
  }

  // Метод формирует JOIN-часть для конкретного включения
  string BuildJoinClause(string include, string mainTableAlias, EntityConfiguration entityConfig)
  {
    var sb = new StringBuilder();
    var relationship = GetRelationship(include);
    var joinType = relationship.IsRequired ? "INNER JOIN" : "LEFT JOIN";

    string joiningTable;
    string joiningTableAlias;
    string fk;
    string pk;

    switch (relationship)
    {
      case OneToManyRelationship oneToMany:
        var principalConfig = _modelBuilder.Configurations[relationship.PrincipalEntity];
        var dependentConfig = _modelBuilder.Configurations[relationship.DependentEntity];

        if (oneToMany.DependentEntity == entityConfig.EntityType)
        {
          joiningTable = principalConfig.TableName;
          fk = oneToMany.ForeignKey ?? $"{principalConfig.TableName}Id";
          pk = principalConfig.PrimaryKey;
        }
        else
        {
          joiningTable = dependentConfig.TableName;
          fk = oneToMany.ForeignKey ?? $"{principalConfig.TableName}Id";
          pk = dependentConfig.PrimaryKey;
        }
        joiningTableAlias = joiningTable.ToLower();

        break;

      case OneToOneRelationship oneToOne:
        var targetConfig = _modelBuilder.Configurations[
            relationship.DependentEntity == entityConfig.EntityType 
                ? relationship.PrincipalEntity 
                : relationship.DependentEntity];

        joiningTable = targetConfig.TableName;
        joiningTableAlias = joiningTable.ToLower();
        fk = oneToOne.ForeignKey ?? $"{targetConfig.TableName}Id";
        pk = targetConfig.PrimaryKey;
        break;

      case ManyToManyRelationship manyToMany:
        joiningTable = manyToMany.JoinEntity.Name;
        joiningTableAlias = joiningTable.ToLower();

        // Обрабатываем составные ключи для M2M
        sb.Append($" {joinType} \"{joiningTable}\" AS {joiningTableAlias}");
        sb.Append($" ON {mainTableAlias}.\"{entityConfig.PrimaryKey}\"");
        sb.Append($" = {joiningTableAlias}.\"{manyToMany.PrincipalForeignKey}\"");
        return sb.ToString();

      default:
        throw new NotSupportedException($"Unsupported relationship type: {relationship.RelationshipType}");
    }

    sb.Append($" {joinType} \"{joiningTable}\" AS {joiningTableAlias}");
    sb.Append($" ON {mainTableAlias}.\"{fk}\" = {joiningTableAlias}.\"{pk}\"");
    return sb.ToString();
  }

  string BuildSelectClause(EntityConfiguration entityConfig, string mainTableAlias)
  {
    var sb = new StringBuilder();

    // Формируем список столбцов для основной сущности, отбирая только "простые" типы.
    var mainProperties = entityConfig.EntityType
                                     .GetProperties()
                                     .Where(p => IsSimpleType(p.PropertyType))
                                     .OrderBy(p => p.Name == "Id" ? 0 : 1) // "Id" всегда первым
                                     .Select(p => p.Name);

    foreach (var col in mainProperties)
    {
      if (sb.Length > 0)
        sb.Append(", ");
      sb.Append($"{mainTableAlias}.\"{col}\"");
    }

    // Для каждого включения (Include) добавляем столбцы из связанной сущности.
    foreach (var include in _includes)
    {
      var relationship = GetRelationship(include);

      // Если основная сущность является зависимой, берем свойства из ее принципала.
      if (relationship.DependentEntity == entityConfig.EntityType)
      {
        var principalConfig = _modelBuilder.Configurations[relationship.PrincipalEntity];
        var alias = principalConfig.TableName
                                   .ToLower();

        var principalProperties = principalConfig.EntityType
                                                 .GetProperties()
                                                 .Where(p => IsSimpleType(p.PropertyType))
                                                 .OrderBy(p => p.Name == "Id" ? 0 : 1)
                                                 .Select(p => p.Name);

        foreach (var col in principalProperties)
        {
          if (sb.Length > 0)
            sb.Append(", ");
          sb.Append($"{alias}.\"{col}\"");
        }
      }
      // Если основная сущность является принципальной, берем свойства из зависимой сущности.
      else if (relationship.PrincipalEntity == entityConfig.EntityType)
      {
        var dependentConfig = _modelBuilder.Configurations[relationship.DependentEntity];
        var alias = dependentConfig.TableName
                                   .ToLower();

        var dependentProperties = dependentConfig.EntityType
                                                 .GetProperties()
                                                 .Where(p => IsSimpleType(p.PropertyType))
                                                 .OrderBy(p => p.Name == "Id" ? 0 : 1)
                                                 .Select(p => p.Name);

        foreach (var col in dependentProperties)
        {
          if (sb.Length > 0)
            sb.Append(", ");
          sb.Append($"{alias}.\"{col}\"");
        }
      }
    }

    return sb.ToString();
  }

  static bool IsSimpleType(Type type)
  {
    var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
    return underlyingType.IsPrimitive || 
           underlyingType.IsEnum ||
           underlyingType == typeof(string) ||
           underlyingType == typeof(decimal) ||
           underlyingType == typeof(DateTime) ||
           underlyingType == typeof(Guid);
  }

  Relationship GetRelationship(string navigationProperty)
    => _modelBuilder.Configurations[typeof(TEntity)]
                    .Relationships
                    .First(r => r.NavigationProperty == navigationProperty);
}
