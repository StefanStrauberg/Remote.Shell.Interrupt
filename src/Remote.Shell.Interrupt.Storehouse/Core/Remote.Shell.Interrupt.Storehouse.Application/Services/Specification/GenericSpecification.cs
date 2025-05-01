namespace Remote.Shell.Interrupt.Storehouse.Application.Services.Specification;

public class GenericSpecification<T> : ISpecification<T> where T : BaseEntity
{

  protected readonly List<Expression<Func<T, object>>> _includes = [];


  protected Expression<Func<T, bool>>? _criteria = null;


  public Expression<Func<T, bool>>? Criterias => _criteria;


  public IReadOnlyCollection<Expression<Func<T, object>>> Includes => _includes;


  public int Take {get; protected set; }


  public int Skip {get; protected set; }


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


  class ReplaceParameterVisitor(ParameterExpression oldParam,
                                ParameterExpression newParam) 
    : ExpressionVisitor
  {
    private readonly ParameterExpression _oldParam = oldParam;
    private readonly ParameterExpression _newParam = newParam;

    protected override Expression VisitParameter(ParameterExpression node)
      => node == _oldParam ? _newParam : node;
  }

  public virtual ISpecification<T> AddInclude(Expression<Func<T, object>> include)
  {
    _includes.Add(include);
    return this;
  }


  public virtual ISpecification<T> WithPagination(int pageNumber, int pageSize)
  {
    if (pageNumber < 1) pageNumber = 1;
    if (pageSize < 1) pageSize = 10;
    
    Skip = (pageNumber - 1) * pageSize;
    Take = pageSize;
    return this;
  }


  public virtual ISpecification<T> Clone()
  {
    var clone = new GenericSpecification<T>
    {
      Take = Take,
      Skip = Skip
    };

    if (_criteria is not null)
      clone.AddFilter(_criteria);

    foreach (var include in _includes)
      clone.AddInclude(include);

    return clone;
  }
}
