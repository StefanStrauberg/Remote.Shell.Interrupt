namespace Remote.Shell.Interrupt.Storehouse.Application.Services.QueryFilterParser;

internal class QueryFilterParser : IQueryFilterParser
{
  Expression<Func<T, bool>>? IQueryFilterParser.ParseFilters<T>(List<FilterDescriptor>? filters)
  {
    if (filters == null || filters.Count == 0)
            return null!;

    Expression<Func<T, bool>>? combinedExpression = null;

    foreach (var filter in filters)
    {
        var filterExpression = filter.ToExpression<T>();
        
        // Комбинируем фильтры через логическое "И" (AND)
        if (combinedExpression == null)
            combinedExpression = filterExpression;
        else
            combinedExpression = CombineExpressions(combinedExpression, filterExpression);
    
    }
    
    return combinedExpression;
  }

  static Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expr1,
                                                         Expression<Func<T, bool>> expr2)
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
