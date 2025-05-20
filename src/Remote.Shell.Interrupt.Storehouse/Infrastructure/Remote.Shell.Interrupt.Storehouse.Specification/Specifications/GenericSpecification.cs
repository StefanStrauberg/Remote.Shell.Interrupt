namespace Remote.Shell.Interrupt.Storehouse.Specification.Specifications;

internal class GenericSpecification<TBase> : ISpecification<TBase> where TBase : BaseEntity
{

  protected readonly List<IIncludeChain<TBase>> _includeChains = [];

  protected Expression<Func<TBase, bool>>? _criteria = null;

  public Expression<Func<TBase, bool>>? Criterias => _criteria;

  public IReadOnlyList<IIncludeChain<TBase>> IncludeChains => [.. _includeChains];

  public int Take { get; protected set; }

  public int Skip { get; protected set; }

  public virtual ISpecification<TBase> AddFilter(Expression<Func<TBase, bool>> criteria)
  {
    if (criteria == null)
      return this;

    if (_criteria == null)
      _criteria = criteria;
    else
      _criteria = CombineExpressions(_criteria, criteria);

    return this;
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
    var chain = new IncludeChain<TBase>();
    chain.AddInclude(include);
    _includeChains.Add(chain);
    return this;
  }

  public ISpecification<TBase> AddThenInclude<TPrevious, TProperty>(Expression<Func<TPrevious, TProperty>> thenInclude)
  {
    if (_includeChains.Count == 0)
      throw new InvalidOperationException("No Include found to apply ThenInclude");

    var lastChain = _includeChains.Last();
    lastChain.AddThenInclude(thenInclude);
    return this;
  }

  public virtual ISpecification<TBase> WithPagination(int pageNumber, int pageSize)
  {
    if (pageNumber < 1) pageNumber = 1;
    if (pageSize < 1) pageSize = 10;

    Skip = (pageNumber - 1) * pageSize;
    Take = pageSize;
    return this;
  }

  public virtual ISpecification<TBase> Clone()
  {
    var clone = new GenericSpecification<TBase>
    {
      Take = Take,
      Skip = Skip,
      _criteria = this._criteria // Expression trees are immutable and safe to reference
    };

    foreach (var chain in this._includeChains)
    {
      var newChain = new IncludeChain<TBase>();
        
      // Copy each include item with type information
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
}
