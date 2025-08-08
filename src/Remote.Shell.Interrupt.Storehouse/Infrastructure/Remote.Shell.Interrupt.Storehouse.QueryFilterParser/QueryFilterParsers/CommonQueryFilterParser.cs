namespace Remote.Shell.Interrupt.Storehouse.QueryFilterParser.QueryFilterParsers;

/// <summary>
/// Parses a list of filter descriptors into a LINQ expression tree.
/// Groups filters by property and combines them using logical AND/OR.
/// </summary>
internal class CommonQueryFilterParser : IQueryFilterParser
{

  /// <summary>
  /// Parses a list of filters into a composed expression tree.
  /// Returns null if filters are empty or null.
  /// </summary>
  /// <typeparam name="T">The target entity type for filtering.</typeparam>
  /// <param name="filters">List of filter descriptors to parse.</param>
  /// <returns>
  /// A composed expression representing the filters, or null if no processing is needed.
  /// </returns>
  Expression<Func<T, bool>>? IQueryFilterParser.ParseFilters<T>(List<FilterDescriptor>? filters)
  {
    if (ShouldSkipProcessing(filters))
      return null;

    var filterGroups = GroupFiltersByProperty(filters!);

    return BuildFinalExpression<T>(filterGroups);
  }

  /// <summary>
  /// Determines whether filter processing should be skipped.
  /// </summary>
  /// <param name="filters">The filter list to evaluate.</param>
  /// <returns>True if filters are null or empty; otherwise false.</returns>
  static bool ShouldSkipProcessing(List<FilterDescriptor>? filters)
    => filters == null || filters.Count == 0;

  /// <summary>
  /// Groups filters by their property path.
  /// </summary>
  /// <param name="filters">List of filters to group.</param>
  /// <returns>Grouped filters by property path.</returns>
  static IEnumerable<IGrouping<string, FilterDescriptor>> GroupFiltersByProperty(List<FilterDescriptor> filters)
    => filters.GroupBy(f => f.PropertyPath);

  /// <summary>
  /// Builds the final expression by combining grouped filter expressions with logical AND.
  /// </summary>
  /// <typeparam name="T">The target entity type.</typeparam>
  /// <param name="filterGroups">Grouped filters by property.</param>
  /// <returns>Composed expression representing all filters.</returns>
  static Expression<Func<T, bool>>? BuildFinalExpression<T>(IEnumerable<IGrouping<string, FilterDescriptor>> filterGroups)
  {
    Expression<Func<T, bool>>? finalExpression = null;

    foreach (var group in filterGroups)
    {
      var groupExpression = BuildGroupExpression<T>(group);
      finalExpression = CombineWithAnd(finalExpression, groupExpression);
    }

    return finalExpression;
  }

  /// <summary>
  /// Builds an expression for a single filter group using logical OR.
  /// </summary>
  /// <typeparam name="T">The target entity type.</typeparam>
  /// <param name="filterGroup">Group of filters for a single property.</param>
  /// <returns>Composed OR expression for the group.</returns>
  static Expression<Func<T, bool>>? BuildGroupExpression<T>(IGrouping<string, FilterDescriptor> filterGroup)
  {
    Expression<Func<T, bool>>? groupExpression = null;

    foreach (var filter in filterGroup)
    {
      var filterExpression = filter.ToExpression<T>();
      groupExpression = CombineWithOr(groupExpression, filterExpression);
    }

    return groupExpression;
  }

  /// <summary>
  /// Combines two expressions using logical AND.
  /// Handles nulls gracefully by short-circuiting.
  /// </summary>
  /// <typeparam name="T">The target entity type.</typeparam>
  /// <param name="left">Left-hand expression.</param>
  /// <param name="right">Right-hand expression.</param>
  /// <returns>Combined expression using AND.</returns>
  static Expression<Func<T, bool>>? CombineWithAnd<T>(Expression<Func<T, bool>>? left,
                                                      Expression<Func<T, bool>>? right)
  {
    if (left == null)
      return right;

    if (right == null)
      return left;

    return CombineExpressions(left, right, Expression.AndAlso);
  }

  /// <summary>
  /// Combines two expressions using logical OR.
  /// Handles nulls gracefully by short-circuiting.
  /// </summary>
  /// <typeparam name="T">The target entity type.</typeparam>
  /// <param name="left">Left-hand expression.</param>
  /// <param name="right">Right-hand expression.</param>
  /// <returns>Combined expression using OR.</returns>
  static Expression<Func<T, bool>>? CombineWithOr<T>(Expression<Func<T, bool>>? left,
                                                     Expression<Func<T, bool>>? right)
  {
    if (left == null)
      return right;
    if (right == null)
      return left;

    return CombineExpressions(left, right, Expression.OrElse);
  }

  /// <summary>
  /// Combines two expressions using a specified binary operator.
  /// Rebinds parameters to a shared context.
  /// </summary>
  /// <typeparam name="T">The target entity type.</typeparam>
  /// <param name="left">Left-hand expression.</param>
  /// <param name="right">Right-hand expression.</param>
  /// <param name="combiner">Binary operator (e.g., AndAlso, OrElse).</param>
  /// <returns>Unified expression with shared parameter.</returns>
  static Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> left,
                                                         Expression<Func<T, bool>> right,
                                                         Func<Expression, Expression, BinaryExpression> combiner)
  {
    var parameter = Expression.Parameter(typeof(T), "x");
    var leftBody = ReplaceParameter(left.Parameters[0], parameter, left.Body);
    var rightBody = ReplaceParameter(right.Parameters[0], parameter, right.Body);
    var combinedBody = combiner(leftBody, rightBody);

    return Expression.Lambda<Func<T, bool>>(combinedBody, parameter);
  }

  /// <summary>
  /// Replaces a parameter in an expression tree with a new one.
  /// </summary>
  /// <param name="oldParameter">Original parameter to replace.</param>
  /// <param name="newParameter">New parameter to inject.</param>
  /// <param name="body">Expression body to rewrite.</param>
  /// <returns>Rewritten expression with new parameter.</returns>
  static Expression ReplaceParameter(ParameterExpression oldParameter,
                                     ParameterExpression newParameter,
                                     Expression body)
    => new ParameterReplacerVisitor(oldParameter, newParameter).Visit(body)!;

  /// <summary>
  /// Visitor that replaces one parameter with another in an expression tree.
  /// </summary>
  class ParameterReplacerVisitor(ParameterExpression oldParameter,
                                 ParameterExpression newParameter)
    : ExpressionVisitor
  {
    readonly ParameterExpression _oldParameter = oldParameter;
    readonly ParameterExpression _newParameter = newParameter;

    protected override Expression VisitParameter(ParameterExpression node)
      => node == _oldParameter ? _newParameter : base.VisitParameter(node);
  }
}
