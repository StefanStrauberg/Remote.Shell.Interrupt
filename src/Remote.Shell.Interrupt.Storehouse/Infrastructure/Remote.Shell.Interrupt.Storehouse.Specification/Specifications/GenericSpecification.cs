namespace Remote.Shell.Interrupt.Storehouse.Specification.Specifications;

/// <summary>
/// Generic specification class for filtering, including related entities, 
/// and applying pagination.
/// </summary>
/// <typeparam name="T">The entity type that this specification operates on.</typeparam>
public class GenericSpecification<T> : ISpecification<T> where T : BaseEntity
{
  /// <summary>
  /// List of expressions for including related entities in queries.
  /// </summary>
  protected readonly List<IIncludeChain<T>> _includeChains = [];

  /// <summary>
  /// Filtering criteria applied to the specification.
  /// </summary>
  protected Expression<Func<T, bool>>? _criteria = null;

  /// <summary>
  /// Gets the filtering criteria applied to the specification.
  /// </summary>
  public Expression<Func<T, bool>>? Criterias => _criteria;

  /// <summary>
  /// Gets the collection of expressions for including related entities.
  /// </summary>
  public IEnumerable<IIncludeChain<T>> IncludeChains => _includeChains;

  /// <summary>
  /// Gets the number of records to take for pagination.
  /// </summary>
  public int Take {get; protected set; }

  /// <summary>
  /// Gets the number of records to skip for pagination.
  /// </summary>
  public int Skip {get; protected set; }

  /// <summary>
  /// Adds a filtering expression to the specification.
  /// </summary>
  /// <param name="criteria">The filtering expression to apply.</param>
  /// <returns>The updated specification instance.</returns>
  public virtual ISpecification<T> AddFilter(Expression<Func<T, bool>> criteria)
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
  /// Combines two filtering expressions using a logical AND operation.
  /// </summary>
  /// <param name="expr1">The first filtering expression.</param>
  /// <param name="expr2">The second filtering expression.</param>
  /// <returns>The combined filtering expression.</returns>
  static Expression<Func<T, bool>> CombineExpressions(Expression<Func<T, bool>> expr1,
                                                      Expression<Func<T, bool>> expr2)
  {
    var parameter = Expression.Parameter(typeof(T));
    var visitor = new ReplaceParameterVisitor(expr1.Parameters[0], parameter);
    var left = visitor.Visit(expr1.Body);
    visitor = new ReplaceParameterVisitor(expr2.Parameters[0], parameter);
    var right = visitor.Visit(expr2.Body);
    return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left!, right!), parameter);
  }

  /// <summary>
  /// Expression visitor for replacing parameter expressions.
  /// </summary>
  class ReplaceParameterVisitor(ParameterExpression oldParam,
                                ParameterExpression newParam) 
    : ExpressionVisitor
  {
    readonly ParameterExpression _oldParam = oldParam;
    readonly ParameterExpression _newParam = newParam;

    /// <summary>
    /// Replaces the parameter expression if it matches the old parameter.
    /// </summary>
    /// <param name="node">The parameter expression to visit.</param>
    /// <returns>The updated parameter expression.</returns>
    protected override Expression VisitParameter(ParameterExpression node)
      => node == _oldParam ? _newParam : node;
  }

  /// <summary>
  /// Adds an expression for including related entities in queries.
  /// </summary>
  /// <param name="include">The include expression.</param>
  /// <returns>The updated specification instance.</returns>
  public virtual ISpecification<T> AddInclude<TProperty>(Expression<Func<T, TProperty>> include)
  {
    var chain = new IncludeChain<T>().AddInclude(include);
    _includeChains.Add(chain);
    return this;
  }

  public ISpecification<T> AddThenInclude<TProperty, TNextProperty>(Expression<Func<T, TProperty>> include,
                                                                    Expression<Func<TProperty, TNextProperty>> thenInclude)
  {
    var chain = new IncludeChain<T>().AddInclude(include).AddThenInclude(thenInclude);
    _includeChains.Add(chain);
    return this;
  }

  public ISpecification<T> AddThenInclude<TCollection, TProperty>(Expression<Func<T, IEnumerable<TCollection>>> collectionInclude,
                                                                  Expression<Func<TCollection, TProperty>> thenInclude)
  {
    var chain = new IncludeChain<T>().AddInclude(collectionInclude).AddThenInclude(thenInclude);
    _includeChains.Add(chain);
    return this;
  }

  /// <summary>
  /// Applies pagination settings to the specification.
  /// </summary>
  /// <param name="pageNumber">The page number.</param>
  /// <param name="pageSize">The number of records per page.</param>
  /// <returns>The updated specification instance.</returns>
  public virtual ISpecification<T> WithPagination(int pageNumber, int pageSize)
  {
    if (pageNumber < 1) pageNumber = 1;
    if (pageSize < 1) pageSize = 10;
    
    Skip = (pageNumber - 1) * pageSize;
    Take = pageSize;
    return this;
  }

  /// <summary>
  /// Creates a copy of the current specification.
  /// </summary>
  /// <returns>A cloned instance of the specification.</returns>
  public virtual ISpecification<T> Clone()
  {
    var clone = new GenericSpecification<T>
    {
      Take = Take,
      Skip = Skip
    };

    if (_criteria is not null)
      clone.AddFilter(_criteria);

    foreach (var includeChain in _includeChains)
      clone._includeChains.Add(new IncludeChain<T>(includeChain.Includes));

    return clone;
  }
}
