namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

public class SQLQueryBuilder<TEntity>(ModelBuilder modelBuilder,
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
    var entityConfig = _modelBuilder.Configurations[typeof(TEntity)];
    var mainTable = entityConfig.TableName;
    var mainTableAlias = mainTable.ToLower();
    var mainPk = entityConfig.PrimaryKey;
    var select = _includes.Count > 0 ? BuildSelectClause(entityConfig, mainTableAlias) 
                                     : (string.IsNullOrEmpty(CustomSelect) ? "*" : CustomSelect);
    var from = string.IsNullOrEmpty(FromClause) ? $"{mainTable}" 
                                                 : FromClause;
    var sql = $"SELECT {select} FROM \"{from}\"";

    foreach (var include in _includes)
    {
      var relationship = GetRelationship(include);
      var joinType = relationship.IsRequired ? "INNER JOIN" : "LEFT JOIN";
      
      if (relationship.IsManyToMany)
      {
        if (relationship.JoinEntity == null)
          throw new InvalidOperationException("JoinEntity not specified for many-to-many relationship");

        var joinConfig = _modelBuilder.Configurations[relationship.JoinEntity];
        var principalConfig = _modelBuilder.Configurations[relationship.PrincipalEntity];
        var dependentConfig = _modelBuilder.Configurations[relationship.DependentEntity];

        sql += $" {joinType} \"{joinConfig.TableName}\" AS jt" +
               $" ON {mainTable}.{mainPk} = jt.{mainTable}Id";

        sql += $" {joinType} {dependentConfig.TableName} AS d" +
               $" ON jt.{dependentConfig.TableName}Id = d.{dependentConfig.PrimaryKey}";
      }
      else
      {
        var principalConfig = _modelBuilder.Configurations[relationship.PrincipalEntity];
        var joiningTable = principalConfig.TableName;
        var joiningTableAlias = principalConfig.TableName.ToLower();
        var fk = relationship.ForeignKey ?? $"{principalConfig.TableName}Id";

        sql += $" {joinType} \"{joiningTable}\" AS {joiningTableAlias}" +
               $" ON {mainTableAlias}.\"{fk}\" = {joiningTableAlias}.\"{principalConfig.PrimaryKey}\"";
      }
    }

    if (!string.IsNullOrEmpty(_whereClause))
      sql += $" WHERE {_whereClause}";

    if (_take.HasValue && _skip.HasValue)
      sql += $" LIMIT {_take.Value} OFFSET {_skip.Value}";

    return sql;
  }

  string BuildSelectClause(EntityConfiguration entityConfig, string mainTableAlias)
  {
    var columns = new List<string>();

    // Добавляем столбцы основной таблицы, например: clients."Id", clients."Name", ...
    foreach (var col in entityConfig.Columns)
      columns.Add($"{mainTableAlias}.\"{col}\"");

    // Если требуется добавить столбцы из join-таблиц, можно перебрать _includes.
    // Здесь можно, например, добавить столбцы из каждой присоединяемой таблицы:
    foreach (var include in _includes)
    {
      var relationship = GetRelationship(include);

      // Если основная сущность (Client) является зависимой в отношении,
      // это значит, что связь настроена как HasOne (например, COD или TfPlan),
      // и связанные столбцы должны браться из принципальной сущности
      if (relationship.DependentEntity == entityConfig.EntityType)
      {
        var principalConfig = _modelBuilder.Configurations[relationship.PrincipalEntity];
        var alias = principalConfig.TableName.ToLower();
        foreach (var col in principalConfig.Columns)
            columns.Add($"{alias}.\"{col}\"");
      }
      // Если же основная сущность является принципальной (например, для коллекционного отношения SPRVlans),
      // то выбираем столбцы из зависимой сущности
      else if (relationship.PrincipalEntity == entityConfig.EntityType)
      {
        var dependentConfig = _modelBuilder.Configurations[relationship.DependentEntity];
        var alias = dependentConfig.TableName.ToLower();
        foreach (var col in dependentConfig.Columns)
            columns.Add($"{alias}.\"{col}\"");
      }
    }

    // Объединяем все столбцы, разделяя запятой.
    return string.Join(", ", columns);
  }

  Relationship GetRelationship(string navigationProperty)
    => _modelBuilder.Configurations[typeof(TEntity)]
                    .Relationships
                    .First(r => r.NavigationProperty == navigationProperty);
}
