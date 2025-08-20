namespace Remote.Shell.Interrupt.Storehouse.QueryFilterParser.QueryFilterParsers;

/// <summary>
/// Parses a list of filter descriptors into a LINQ expression tree.
/// Groups filters by property and combines them using logical AND/OR.
/// </summary>
internal class CommonQueryFilterParser : IQueryFilterParser
{

  // <inheritdoc />
  Expression<Func<T, bool>>? IQueryFilterParser.ParseFilters<T>(List<FilterDescriptor>? filters)
  {
    if (ShouldSkipProcessing(filters))
      return null;

    var filterGroups = GroupFiltersByProperty(filters!);

    return BuildFilterExpression<T>(filterGroups);
  }

  /// <inheritdoc />
  Expression<Func<T, object>>? IQueryFilterParser.ParseOrderBy<T>(string? propertyName)
  {
    if (ShouldSkipProcessing(propertyName))
      return null;

    return BuildOrderByExpression<T>(propertyName!);
  }

  /// <summary>
  /// Determines whether filter processing should be skipped.
  /// </summary>
  static bool ShouldSkipProcessing(List<FilterDescriptor>? filters)
    => filters == null || filters.Count == 0;

  static bool ShouldSkipProcessing(string? propertyName)
    => string.IsNullOrWhiteSpace(propertyName);

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
  static Expression<Func<T, bool>>? BuildFilterExpression<T>(IEnumerable<IGrouping<string, FilterDescriptor>> filterGroups)
  {
    Expression<Func<T, bool>>? finalExpression = null;

    foreach (var group in filterGroups)
    {
      var groupExpression = BuildGroupExpression<T>(group);
      finalExpression = CombineWithAnd(finalExpression, groupExpression);
    }

    return finalExpression;
  }

  static Expression<Func<T, object>>? BuildOrderByExpression<T>(string propertyName)
  {
    var parameter = Expression.Parameter(typeof(T), "entity");
    Expression propertyExpression = parameter;
    
    foreach (var member in propertyName!.Split('.'))
      propertyExpression = Expression.PropertyOrField(propertyExpression, member);

    var converted = Expression.Convert(propertyExpression, typeof(object));

    return Expression.Lambda<Func<T, object>>(converted, parameter);
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
