namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Helpers;

internal class SqlQueryBuilderUpdated<T> where T : BaseEntity
{
    private readonly List<string> joins = [];
    private readonly List<string> whereClauses = [];
    private readonly DynamicParameters parameters = new();
    private readonly int? take = null;
    private readonly int? skip = null;

    // Для join-сущностей создаём список, содержащий alias и тип присоединенной сущности.
    private readonly List<(string Alias, Type JoinType)> joinSelections = [];

    public SqlQueryBuilderUpdated<T> AddJoin<TJoin>(string joinClause, string alias) where TJoin : BaseEntity
    {
        joins.Add(joinClause);
        joinSelections.Add((alias, typeof(TJoin)));
        return this;
    }

    public SqlQueryBuilderUpdated<T> AddFilter(string condition, object value, string paramName)
    {
        whereClauses.Add(condition);
        parameters.Add(paramName, value);
        return this;
    }

    public (string Sql, DynamicParameters Parameters) Build()
    {
        var sql = new StringBuilder();
        
        // Формируем SELECT с полями основной сущности и join-сущностей.
        sql.Append(BuildSelectClause());

        // FROM с главной таблицей.
        sql.AppendLine($"FROM \"{GetTableName.Handle<T>()}\" AS {typeof(T).Name.ToLower()}");
        
        foreach (var join in joins)
            sql.AppendLine(join);

        if (whereClauses.Count > 0)
            sql.AppendLine("WHERE " + string.Join(" AND ", whereClauses));

        if (take.HasValue)
            sql.AppendLine($"LIMIT {take}");

        if (skip.HasValue)
            sql.AppendLine($"OFFSET {skip}");

        return (sql.ToString(), parameters);
    }

    private string BuildSelectClause()
    {
        List<string> selectParts = [];

        selectParts.Add(BuildSelectClauseMain());

        foreach (var (alias, joinType) in joinSelections)
            selectParts.Add(BuildSelectClauseJoin(alias, joinType));

        return "SELECT " + string.Join(",\n", selectParts);
    }

    private static string BuildSelectClauseMain()
    {
        // Фильтруем свойства по типу. Можно доработать по необходимости.
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                  .Where(p => SqlQueryBuilderUpdated<T>.IsSimpleType(p.PropertyType));

        // Формируем список: prefix."PropertyName"
        var prefix = typeof(T).Name.ToLower();
        var selectColumns = properties.Select(p => $"{prefix}.\"{p.Name}\"");
        return string.Join(",\n", selectColumns);
    }

    private static string BuildSelectClauseJoin(string alias, Type joinType)
    {
        var properties = joinType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .Where(p => IsSimpleType(p.PropertyType));

        // Формируем список вида: alias."PropertyName"
        var selectColumns = properties.Select(p => $"{alias}.\"{p.Name}\"");
        return string.Join(",\n", selectColumns);
    }

    private static bool IsSimpleType(Type type)
    {
        return type.IsPrimitive 
            || new[] { typeof(string), typeof(decimal), typeof(DateTime), typeof(DateTimeOffset), typeof(Guid) }.Contains(type)
            || type.IsEnum;
    }
}
