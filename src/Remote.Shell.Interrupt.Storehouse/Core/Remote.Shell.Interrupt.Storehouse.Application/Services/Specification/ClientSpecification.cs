namespace Remote.Shell.Interrupt.Storehouse.Application.Services.Specification;

/// <summary>
/// Represents a specification for querying <see cref="Client"/> entities with filters, includes, and pagination.
/// </summary>
public class ClientSpecification : IClientSpecification
{
  /// <summary>
  /// Stores the include expressions for navigation properties.
  /// </summary>
  readonly List<Expression<Func<Client, object>>> _includes = [];

  /// <summary>
  /// Stores the criteria for filtering entities.
  /// </summary>
  Expression<Func<Client, bool>>? _criteria = null;

  /// <summary>
  /// Gets the filtering criteria expression.
  /// </summary>
  Expression<Func<Client, bool>>? ISpecification<Client>.Criterias => _criteria;

  /// <summary>
  /// Gets the include expressions for navigation properties as a read-only collection.
  /// </summary>
  IReadOnlyCollection<Expression<Func<Client, object>>> ISpecification<Client>.Includes => _includes.AsReadOnly();

  /// <summary>
  /// Gets the maximum number of entities to take for pagination.
  /// </summary>
  public int Take {get; private set; } = 0;

  /// <summary>
  /// Gets the number of entities to skip for pagination.
  /// </summary>
  public int Skip {get; private set; } = 0;

  /// <summary>
  /// Adds a filtering criterion to the specification.
  /// </summary>
  /// <param name="criteria">The filtering expression to add.</param>
  /// <returns>The updated specification with the filter added.</returns>
  ISpecification<Client> ISpecification<Client>.AddFilter(Expression<Func<Client, bool>> criteria)
  {
    if (criteria == null)
      return this;
    
    if (_criteria == null)
      _criteria = criteria;
    else
      _criteria = CombineExpressions(_criteria, criteria);

    return this;
  }

  /// <summary>
  /// Combines two filtering expressions using a logical AND.
  /// </summary>
  /// <param name="expr1">The first expression.</param>
  /// <param name="expr2">The second expression.</param>
  /// <returns>A combined expression.</returns>
  static Expression<Func<Client, bool>> CombineExpressions(Expression<Func<Client, bool>> expr1,
                                                           Expression<Func<Client, bool>> expr2)
  {
      var parameter = Expression.Parameter(typeof(Client));
      var visitor = new ReplaceParameterVisitor(expr1.Parameters[0], parameter);
      var left = visitor.Visit(expr1.Body);
      visitor = new ReplaceParameterVisitor(expr2.Parameters[0], parameter);
      var right = visitor.Visit(expr2.Body);
      return Expression.Lambda<Func<Client, bool>>(Expression.AndAlso(left!, right!), parameter);
  }

  /// <summary>
  /// A helper class to replace parameters in LINQ expressions.
  /// </summary>
  /// <param name="oldParam">The parameter to replace.</param>
  /// <param name="newParam">The new parameter.</param>
  class ReplaceParameterVisitor(ParameterExpression oldParam,
                                ParameterExpression newParam) 
    : ExpressionVisitor
  {
    private readonly ParameterExpression _oldParam = oldParam;
    private readonly ParameterExpression _newParam = newParam;

    /// <summary>
    /// Visits the parameter and replaces it if it matches the old parameter.
    /// </summary>
    /// <param name="node">The parameter expression to visit.</param>
    /// <returns>The updated parameter expression.</returns>
    protected override Expression VisitParameter(ParameterExpression node)
      => node == _oldParam ? _newParam : node;
  }

  /// <summary>
  /// Adds an include expression for navigation properties.
  /// </summary>
  /// <param name="include">The include expression to add.</param>
  /// <returns>The updated specification with the include added.</returns>
  ISpecification<Client> ISpecification<Client>.AddInclude(Expression<Func<Client, object>> include)
  {
    _includes.Add(include);
    return this;
  }

  /// <summary>
  /// Configures the specification with pagination parameters.
  /// </summary>
  /// <param name="pageNumber">The page number (1-based).</param>
  /// <param name="pageSize">The size of the page.</param>
  /// <returns>The updated specification with pagination settings.</returns>
  ISpecification<Client> ISpecification<Client>.WithPagination(int pageNumber, int pageSize)
  {
    if (pageNumber < 1) pageNumber = 1;
    if (pageSize < 1) pageSize = 10;
    
    Skip = (pageNumber - 1) * pageSize;
    Take = pageSize;
    return this;
  }

  /// <summary>
  /// Creates a clone of the current specification.
  /// </summary>
  /// <returns>A new instance of <see cref="ClientSpecification"/> with the same data.</returns>
  object ICloneable.Clone()
  {
    var clone = new ClientSpecification
    {
      Take = Take,
      Skip = Skip
    };

    if (_criteria is not null)
      ((ISpecification<Client>)clone).AddFilter(_criteria);

    foreach (var include in _includes)
      ((ISpecification<Client>)clone).AddInclude(include);

    return clone;
  }
}
