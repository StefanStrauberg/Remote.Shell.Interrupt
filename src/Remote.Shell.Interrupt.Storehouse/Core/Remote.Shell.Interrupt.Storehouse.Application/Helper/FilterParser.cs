namespace Remote.Shell.Interrupt.Storehouse.Application.Helper;

public static class FilterParser
{
    public static Expression<Func<T, bool>>? ParseFilters<T>(List<FilterDescriptor>? filters)
    {
        if (filters == null || filters.Count == 0)
            return null;

        Expression<Func<T, bool>>? combinedExpression = null;

        foreach (var filter in filters)
        {
            var filterExpression = filter.ToExpression<T>();

            // Комбинируем фильтры через логическое "И" (AND)
            if (combinedExpression == null)
            {
                combinedExpression = filterExpression;
            }
            else
            {
                combinedExpression = CombineExpressions(combinedExpression, filterExpression);
            }
        }

        return combinedExpression;
    }

    public static Expression<Func<T, bool>> ToExpression<T>(this FilterDescriptor filter)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = filter.PropertyPath.Split('.')
                                           .Aggregate((Expression)parameter,
                                                      Expression.Property);
        
        // Обрабатываем оператор Contains отдельно: приводим значение к нижнему регистру заранее
        Expression comparison;
        
        if (filter.Operator == FilterOperator.Contains)
        {
            if (property.Type != typeof(string))
                throw new ArgumentException("Contains operator can only be used with string properties");

            // Приводим значение к строке и ниже регистр
            var stringValue = filter.Value.ToString()?.ToLower();
            // Создаем константное выражение для значения уже в нижнем регистре
            var value = Expression.Constant(stringValue);
            var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
            var propertyToLower = Expression.Call(property, toLowerMethod);
            // Теперь вызываем Contains без дополнительного вызова ToLower для value, т.к. оно уже приведено
            var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;
            comparison = Expression.Call(propertyToLower, containsMethod, value);
        }
        else
        {
            var value = Expression.Constant(filter.Value);
            comparison = filter.Operator switch
            {
                FilterOperator.Equals => Expression.Equal(property, value),
                FilterOperator.NotEquals => Expression.NotEqual(property, value),
                FilterOperator.GraterThan => Expression.GreaterThan(property, value),
                FilterOperator.LessThan => Expression.LessThan(property, value),
                _ => throw new NotImplementedException($"Operator {filter.Operator} not supported.")
            };
        }

        return Expression.Lambda<Func<T, bool>>(comparison, parameter);
    }

    private static Expression CreateContainsExpression(Expression property, Expression value)
    {
        if (property.Type != typeof(string))
            throw new ArgumentException("Contains operator can only be used with string properties");

        // Приведение к нижнему регистру
        var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);
        var containsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)]);

        var propertyToLower = Expression.Call(property, toLowerMethod!);
        var valueToLower = Expression.Call(value, toLowerMethod!);

        // Выражение property.ToLower().Contains(value.ToLower())
        return Expression.Call(propertyToLower, containsMethod!, valueToLower);
    }

    private static Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var parameter = expr1.Parameters[0];

        var leftVisitor = new ReplaceParameterVisitor(expr1.Parameters[0], parameter);
        var rightVisitor = new ReplaceParameterVisitor(expr2.Parameters[0], parameter);

        var left = leftVisitor.Visit(expr1.Body);
        var right = rightVisitor.Visit(expr2.Body);

        var combined = Expression.AndAlso(left!, right!);
        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }

    private class ReplaceParameterVisitor(ParameterExpression oldParameter,
                                          ParameterExpression newParameter) : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter = oldParameter;
        private readonly ParameterExpression _newParameter = newParameter;

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParameter ? _newParameter : base.VisitParameter(node);
        }
    }
}
