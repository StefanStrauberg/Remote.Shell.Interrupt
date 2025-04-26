namespace Remote.Shell.Interrupt.Storehouse.Application.Services.Specification;

public class ClientSpecification : IClientSpecification
{
  private readonly List<Expression<Func<Client, object>>> _includes = [];

  public Expression<Func<Client, bool>>? Criteria {get; private set; }

  public IReadOnlyList<Expression<Func<Client, object>>> Includes => _includes.AsReadOnly();

  public int Take {get; private set; } = 0;

  public int Skip {get; private set; } = 0;

  public ISpecification<Client> AddFilter(Expression<Func<Client, bool>> criteria)
  {
    if (Criteria == null)
    {
        Criteria = criteria;
    }
    else
    {
        var combined = CombineExpressions(Criteria, criteria);
        Criteria = combined;
    }
    return this;
  }

  private static Expression<Func<Client, bool>> CombineExpressions(Expression<Func<Client, bool>> expr1,
                                                                   Expression<Func<Client, bool>> expr2)
  {
      var parameter = Expression.Parameter(typeof(Client));
      var visitor = new ReplaceParameterVisitor(expr1.Parameters[0], parameter);
      var left = visitor.Visit(expr1.Body);
      visitor = new ReplaceParameterVisitor(expr2.Parameters[0], parameter);
      var right = visitor.Visit(expr2.Body);
      return Expression.Lambda<Func<Client, bool>>(Expression.AndAlso(left!, right!), parameter);
  }

  private class ReplaceParameterVisitor(ParameterExpression oldParam,
                                        ParameterExpression newParam) 
    : ExpressionVisitor
  {
      private readonly ParameterExpression _oldParam = oldParam;
      private readonly ParameterExpression _newParam = newParam;

    protected override Expression VisitParameter(ParameterExpression node)
          => node == _oldParam ? _newParam : node;
  }

  public ISpecification<Client> AddInclude(Expression<Func<Client, object>> include)
  {
    _includes.Add(include);
    return this;
  }

  public ISpecification<Client> WithPagination(int pageNumber, int pageSize)
  {
    if (pageNumber < 1) pageNumber = 1;
    if (pageSize < 1) pageSize = 10;
    
    Skip = (pageNumber - 1) * pageSize;
    Take = pageSize;
    return this;
  }
}
