namespace Remote.Shell.Interrupt.Storehouse.QueryFilterParser.QueryFilterParsers;

/// <summary>
/// Provides functionality to parse filter descriptors into LINQ expressions for filtering data.
/// </summary>
internal class CommonQueryFilterParser : IQueryFilterParser
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

    // Группируем фильтры по одному и тому же свойству (PropertyPath)
    var groupedFilters = filters.GroupBy(f => f.PropertyPath);

    Expression<Func<T, bool>>? finalExpression = null;

    foreach (var group in groupedFilters)
    {
      // Внутри группы условие объединяем через OR
      Expression<Func<T, bool>>? groupExpression = null;

      foreach (var filter in group)
      {
        var filterExpr = filter.ToExpression<T>();

        if (groupExpression == null)
          groupExpression = filterExpr;
        else
          groupExpression = CombineExpressionsOr(groupExpression, filterExpr);
      }

      // Объединяем полученные групповые выражения через AND
      if (groupExpression != null)
      {
        if (finalExpression == null)
          finalExpression = groupExpression;
        else
          finalExpression = CombineExpressionsAnd(finalExpression, groupExpression);
      }
    }
    
    return finalExpression;
  }

  static Expression<Func<T, bool>> CombineExpressionsAnd<T>(Expression<Func<T, bool>> expr1,
                                                            Expression<Func<T, bool>> expr2)
  {
    var parameter = Expression.Parameter(typeof(T), "x");
    var left = new ParameterReplacerVisitor(expr1.Parameters[0], parameter).Visit(expr1.Body);
    var right = new ParameterReplacerVisitor(expr2.Parameters[0], parameter).Visit(expr2.Body);
    var body = Expression.AndAlso(left!, right!);
    return Expression.Lambda<Func<T, bool>>(body, parameter);
  }

  static Expression<Func<T, bool>> CombineExpressionsOr<T>(Expression<Func<T, bool>> expr1,
                                                           Expression<Func<T, bool>> expr2)
  {
    var parameter = Expression.Parameter(typeof(T), "x");
    var left = new ParameterReplacerVisitor(expr1.Parameters[0], parameter).Visit(expr1.Body);
    var right = new ParameterReplacerVisitor(expr2.Parameters[0], parameter).Visit(expr2.Body);
    var body = Expression.OrElse(left!, right!);
    return Expression.Lambda<Func<T, bool>>(body, parameter);
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
    readonly ParameterExpression _oldParameter = oldParameter;
    readonly ParameterExpression _newParameter = newParameter;

    /// <summary>
    /// Visits the parameter and replaces it if it matches the old parameter.
    /// </summary>
    /// <param name="node">The parameter expression to visit.</param>
    /// <returns>The updated parameter expression.</returns>
    protected override Expression VisitParameter(ParameterExpression node)
      => node == _oldParameter ? _newParameter : base.VisitParameter(node);
  }
}
