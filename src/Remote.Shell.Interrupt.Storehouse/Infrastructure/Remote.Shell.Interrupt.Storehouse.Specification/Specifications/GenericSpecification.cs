namespace Remote.Shell.Interrupt.Storehouse.Specification.Specifications;

internal class GenericSpecification<TBase> : ISpecification<TBase> where TBase : BaseEntity
{
  // Includes
  protected readonly List<IIncludeChain<TBase>> _includeChains = [];

  // Filters
  protected Expression<Func<TBase, bool>>? _criteria;

  // Filtered Includes
  protected readonly List<LambdaExpression> _filteredIncludeChains = [];

  public IReadOnlyList<LambdaExpression> FilteredIncludeChains => _filteredIncludeChains.AsReadOnly();

  // OrderBy
  protected Expression<Func<TBase, object>>? _orderBy;
  protected Expression<Func<TBase, object>>? _orderByDescending;

  // Properties
  public Expression<Func<TBase, bool>>? Criterias => _criteria;

  public IReadOnlyList<IIncludeChain<TBase>> IncludeChains => _includeChains.AsReadOnly();

  public Expression<Func<TBase, object>>? OrderBy => _orderBy;
  public Expression<Func<TBase, object>>? OrderByDescending => _orderByDescending;

  public int Take { get; protected set; }

  public int Skip { get; protected set; }

  internal const int MaxPageSize = 1000;

  public virtual ISpecification<TBase> AddFilter(Expression<Func<TBase, bool>> criteria)
  {
    if (criteria is null)
      return this;

    _criteria = _criteria is null ? criteria : CombineExpressions(_criteria, criteria);

    return this;
  }

  public virtual ISpecification<TBase> AddOrderBy<TKey>(Expression<Func<TBase, TKey>> orderByExpression)
  {
    if (orderByExpression is not null)
    {
      _orderBy = ConvertToObjectExpression(orderByExpression);
      _orderByDescending = null;
    }

    return this;
  }

  public virtual ISpecification<TBase> AddOrderByDescending<TKey>(Expression<Func<TBase, TKey>> orderByExpression)
  {
    if (orderByExpression != null)
    {
      _orderByDescending = ConvertToObjectExpression(orderByExpression);
      _orderBy = null;
    }

    return this;
  }

  static Expression<Func<TBase, object>> ConvertToObjectExpression<TKey>(Expression<Func<TBase, TKey>> expression)
  {
    var param = expression.Parameters[0];
    var body = Expression.Convert(expression.Body, typeof(object));

    return Expression.Lambda<Func<TBase, object>>(body, param);
  }

  static Expression<Func<TBase, bool>> CombineExpressions(Expression<Func<TBase, bool>> expr1,
                                                          Expression<Func<TBase, bool>> expr2)
  {
    var parameter = Expression.Parameter(typeof(TBase));
    var visitor = new ReplaceParameterVisitor(expr1.Parameters[0], parameter);
    var left = visitor.Visit(expr1.Body);
    visitor = new ReplaceParameterVisitor(expr2.Parameters[0], parameter);
    var right = visitor.Visit(expr2.Body);
    return Expression.Lambda<Func<TBase, bool>>(Expression.AndAlso(left!, right!), parameter);
  }

  class ReplaceParameterVisitor(ParameterExpression oldParam, ParameterExpression newParam)
    : ExpressionVisitor
  {
    readonly ParameterExpression _oldParam = oldParam;
    readonly ParameterExpression _newParam = newParam;
    protected override Expression VisitParameter(ParameterExpression node)
      => node == _oldParam ? _newParam : node;
  }

  public virtual ISpecification<TBase> AddInclude<TProperty>(Expression<Func<TBase, TProperty>> include)
  {
    if (include is null)
      throw new ArgumentNullException(nameof(include), "Include expression cannot be null.");

    var chain = new IncludeChain<TBase>();
    chain.AddInclude(include);
    _includeChains.Add(chain);
    return this;
  }

  public virtual ISpecification<TBase> AddThenInclude<TPrevious, TProperty>(Expression<Func<TPrevious, TProperty>> thenInclude)
  {
    if (_includeChains.Count == 0)
      throw new InvalidOperationException("No Include found to apply ThenInclude");

    var lastChain = _includeChains.Last();
    lastChain.AddThenInclude(thenInclude);
    return this;
  }

  public virtual ISpecification<TBase> ConfigurePagination(PaginationContext paginationContext)
  {
    if (paginationContext is null)
      throw new ArgumentNullException(nameof(paginationContext), "PaginationContext cannot be null.");

    int pageNumber = paginationContext.PageNumber < 1 ? 1 : paginationContext.PageNumber;
    int pageSize = paginationContext.PageSize < 1 ? 10 : paginationContext.PageSize;

    pageSize = Math.Min(pageSize, MaxPageSize);
    long skipLong = ((long)pageNumber - 1) * pageSize;

    Skip = skipLong > int.MaxValue ? int.MaxValue : (int)skipLong;
    Take = pageSize;
    return this;
  }

  public virtual ISpecification<TBase> Clone()
  {
    var clone = new GenericSpecification<TBase>
    {
      Take = Take,
      Skip = Skip,
      _criteria = _criteria,
      _orderBy = _orderBy,
      _orderByDescending = _orderByDescending
    };

    foreach (var chain in _includeChains)
    {
      var newChain = new IncludeChain<TBase>();

      foreach (var include in chain.Includes)
      {
        var method = typeof(IncludeChain<TBase>).GetMethod(nameof(IncludeChain<TBase>.AddTypedInclude))!
                                                .MakeGenericMethod(include.EntityType,
                                                                   include.PropertyType);

        method.Invoke(newChain, [include.Expression]);
      }

      clone._includeChains.Add(newChain);
    }

    return clone;
  }
  
  public virtual ISpecification<TBase> AddFilteredInclude<TProperty>(Expression<Func<TBase, IEnumerable<TProperty>>> includeExpression)
  {
    // Convert the expression to the appropriate format for filtered includes
    var parameter = includeExpression.Parameters[0];
    var body = includeExpression.Body;
      
    // Create a new expression that can be handled by your existing infrastructure
    var newExpression = Expression.Lambda(body, parameter);
    _filteredIncludeChains.Add(newExpression);
      
    return this;
  }
}
