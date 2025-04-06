namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Helpers;

public partial class SqlQueryBuilder
{
    private readonly RequestParameters _request;
    private readonly string? _tableAlias;
    private readonly Dictionary<string, PropertyInfo> _propertyMap;

    public SqlQueryBuilder(RequestParameters request,
                                     string tableAlias,
                                     Type entityType)
    {
        _request = request
            ?? throw new ArgumentNullException(nameof(request));
        _tableAlias = tableAlias
            ?? throw new ArgumentNullException(nameof(tableAlias));
        _propertyMap = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .ToDictionary(p => p.Name,
                                               p => p,
                                               StringComparer.OrdinalIgnoreCase)
            ?? throw new ArgumentNullException(nameof(entityType));
    }

    public SqlQueryBuilder(RequestParameters request,
                                     Type entityType)
    {
        _request = request
            ?? throw new ArgumentNullException(nameof(request));
        _tableAlias = null;
        _propertyMap = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .ToDictionary(p => p.Name,
                                               p => p,
                                               StringComparer.OrdinalIgnoreCase)
            ?? throw new ArgumentNullException(nameof(entityType));
    }

    public (string Sql, DynamicParameters Parameters) BuildBaseQuery(string baseSelect)
    {
        var sb = new StringBuilder(baseSelect);
        var parameters = new DynamicParameters();

        // WHERE 1=1
        sb.Append(" WHERE 1=1 ");

        // FILTERS
        if (!string.IsNullOrWhiteSpace(_request.Filters))
        {
            var filters = _request.Filters.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (var filter in filters)
            {
                var match = MyRegex().Match(filter);

                if (match.Success)
                {
                    var field = match.Groups[1].Value.Trim();
                    var op = match.Groups[2].Value;
                    var value = match.Groups[3].Value.Trim();

                    if (_propertyMap.TryGetValue(field, out var property))
                    {
                        var paramName = $"@{field}_{Guid.NewGuid().ToString("N")[..6]}";
                        string sqlOp = op switch
                        {
                            "==" => "=",
                            "!=" => "<>",
                            "~=" => "ILIKE",
                            _ => op
                        };

                        object typedValue = Convert.ChangeType(value,
                                                               Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);

                        if (sqlOp == "ILIKE")
                            typedValue = $"%{value}%";

                        if (_tableAlias is null)
                        {
                            sb.Append(sqlOp == "ILIKE"
                                    ? $"AND \"{property.Name}\" ILIKE {paramName} "
                                    : $"AND \"{property.Name}\" {sqlOp} {paramName} ");
                        } 
                        else
                        {
                            sb.Append(sqlOp == "ILIKE"
                                    ? $"AND {_tableAlias}.\"{property.Name}\" ILIKE {paramName} "
                                    : $"AND {_tableAlias}.\"{property.Name}\" {sqlOp} {paramName} ");
                        }


                        parameters.Add(paramName, typedValue);
                    }
                }
            }
        }

        // SORTING
        if (!string.IsNullOrWhiteSpace(_request.Sorts))
        {
            var sortField = _request.Sorts.TrimStart('-');
            var direction = _request.Sorts.StartsWith('-') ? "DESC" : "ASC";

            if (_propertyMap.TryGetValue(sortField, out var property))
            {
                sb.Append($" ORDER BY {_tableAlias}.\"{property.Name}\" {direction} ");
            }
        }

        // PAGINATION
        var offset = (_request.PageNumber - 1) * _request.PageSize;
        sb.Append(" LIMIT @Limit OFFSET @Offset ");
        parameters.Add("Limit", _request.PageSize);
        parameters.Add("Offset", offset);

        return (sb.ToString(), parameters);
    }

    [GeneratedRegex(@"(\w+)(==|!=|>=|<=|>|<|~=)(.+)")]
    private static partial Regex MyRegex();
}
