namespace Remote.Shell.Interrupt.Storehouse.Application.Services.QueryFilterParser;

/// <summary>
/// Provides functionality to parse filter descriptors into LINQ expressions for filtering data.
/// </summary>
internal class QueryFilterParser : IQueryFilterParser
{
  /// <summary>
  /// Parses a list of filter descriptors into a combined LINQ expression using logical "AND".
  /// </summary>
  /// <typeparam name="T">The type of entity to which the filters apply.</typeparam>
  /// <param name="filters">A list of <see cref="FilterDescriptor"/> objects to parse.</param>
  /// <returns>
  /// A LINQ expression combining the filters, or <c>null</c> if the filter list is empty or invalid.
  /// </returns>
  Expression<Func<T, bool>>? IQueryFilterParser.ParseFilters<T>(List<FilterDescriptor>? filters)
  {
    if (filters == null || filters.Count == 0)
            return null!;

    Expression<Func<T, bool>>? combinedExpression = null;

    foreach (var filter in filters)
    {
        var filterExpression = filter.ToExpression<T>();
        
        // Комбинируем фильтры через логическое "И" (AND)
        if (combinedExpression is null)
            combinedExpression = filterExpression;
        else
            combinedExpression = CombineExpressions(combinedExpression, filterExpression);
    
    }
    
    return combinedExpression;
  }

  /// <summary>
  /// Combines two LINQ expressions using logical "AND".
  /// </summary>
  /// <typeparam name="T">The type of entity to which the filters apply.</typeparam>
  /// <param name="expr1">The first expression.</param>
  /// <param name="expr2">The second expression.</param>
  /// <returns>
  /// A new LINQ expression that combines the two filters.
  /// </returns>
  static Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expr1,
                                                         Expression<Func<T, bool>> expr2)
  {
      var parameter = expr1.Parameters[0];

      var leftVisitor = new ParameterReplacerVisitor(expr1.Parameters[0], parameter);
      var rightVisitor = new ParameterReplacerVisitor(expr2.Parameters[0], parameter);

      var left = leftVisitor.Visit(expr1.Body);
      var right = rightVisitor.Visit(expr2.Body);

      var combined = Expression.AndAlso(left!, right!);
      return Expression.Lambda<Func<T, bool>>(combined, parameter);
  }

  /// <summary>
  /// A helper class to replace parameters in LINQ expressions.
  /// </summary>
  /// <param name="oldParameter">The parameter to replace.</param>
  /// <param name="newParameter">The new parameter.</param>
  class ParameterReplacerVisitor(ParameterExpression oldParameter,
                                 ParameterExpression newParameter) 
    : ExpressionVisitor
  {
      private readonly ParameterExpression _oldParameter = oldParameter;
      private readonly ParameterExpression _newParameter = newParameter;

      /// <summary>
      /// Visits the parameter and replaces it if it matches the old parameter.
      /// </summary>
      /// <param name="node">The parameter expression to visit.</param>
      /// <returns>The updated parameter expression.</returns>
      protected override Expression VisitParameter(ParameterExpression node)
      {
          return node == _oldParameter ? _newParameter : base.VisitParameter(node);
      }
  }
}
