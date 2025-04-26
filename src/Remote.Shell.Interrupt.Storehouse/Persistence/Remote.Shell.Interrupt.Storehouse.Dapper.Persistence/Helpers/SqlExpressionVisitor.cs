namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Helpers;

internal class SqlExpressionVisitor<T> : ExpressionVisitor
{
    public readonly Dictionary<string, object> Parameters = [];
    private int _parameterIndex = 0;
    private readonly StringBuilder _queryBuilder = new();

    public string GetWhereClause(Expression<Func<T, bool>> criteria)
    {
        Visit(criteria.Body);
        return _queryBuilder.ToString();
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        // Обрабатываем бинарные выражения (например, x => x.Id == 1)
        _queryBuilder.Append('(');
        Visit(node.Left);
        _queryBuilder.Append(GetSqlOperator(node.NodeType));
        Visit(node.Right);
        _queryBuilder.Append(')');
        return node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        // Обрабатываем доступ к свойствам (например, x.Id)
        _queryBuilder.Append($"{typeof(T).Name.ToLower()}.\"{node.Member.Name}\"");
        return node;
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        // Добавляем параметры
        var paramName = $"@p{_parameterIndex++}";
        _queryBuilder.Append(paramName);
        Parameters[paramName] = node.Value!;
        return node;
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        // Обрабатываем вызовы метода, например для Contains
        if (node.Method.Name == "Contains" && node.Object != null)
        {
            Visit(node.Object);

            // Добавляем ILIKE вместо стандартного оператора
            _queryBuilder.Append(" ILIKE ");
            
            // Обрабатываем аргументы метода.
            // Если значение передано как константа, оно оборачивается в шаблон для поиска.
            if (node.Arguments[0] is ConstantExpression constant)
            {
                var value = constant.Value?.ToString() ?? string.Empty;
                var paramName = $"@p{_parameterIndex++}";
                _queryBuilder.Append(paramName);
                // Оборачиваем значение в шаблоны %value%
                Parameters[paramName] = $"%{value}%";
            }
            else
            {
                Visit(node.Arguments[0]);
            }

            return node;
        }
        // Для остальных методов можно вызвать базовую реализацию или выбросить исключение.
        return base.VisitMethodCall(node);
    }

    private static string GetSqlOperator(ExpressionType nodeType)
    {
        return nodeType switch
        {
            ExpressionType.Equal => " = ",
            ExpressionType.NotEqual => " <> ",
            ExpressionType.GreaterThan => " > ",
            ExpressionType.LessThan => " < ",
            ExpressionType.AndAlso => " AND ",
            _ => throw new NotImplementedException($"Operator {nodeType} not supported.")
        };
    }
}
