namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Specification;

internal class ClientSpecification : ISpecification<Client>
{
  private readonly List<Expression<Func<Client, object>>> _includes = [];

  public Expression<Func<Client, bool>>? Criteria { get; private set; }
  public IReadOnlyList<Expression<Func<Client, object>>> Includes => _includes.AsReadOnly();
  public int Take { get; private set; } = 0;
  public int Skip { get; private set; } = 0;

  // Метод для фильтрации
  public ClientSpecification AddFilter(Expression<Func<Client, bool>> filter)
  {
    Criteria = Criteria == null ? filter 
                                : Expression.Lambda<Func<Client, bool>>(Expression.AndAlso(Criteria.Body, filter.Body),
                                                                        Criteria.Parameters[0]);
    return this;
  }

  // Метод для включения связанных данных
  public ClientSpecification AddInclude(Expression<Func<Client, object>> include)
  {
    _includes.Add(include);
    return this;
  }

  // Метод для пагинации
  public ClientSpecification WithPagination(int pageNumber, int pageSize)
  {
    Skip = (pageNumber - 1) * pageSize;
    Take = pageSize;
    return this; // Возвращаем текущий экземпляр для цепочки вызовов
  }

  private static Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expr1,
                                                                 Expression<Func<T, bool>> expr2)
  {
    var parameter = expr1.Parameters[0];
    var leftVisitor = new ReplaceParameterVisitor(expr1.Parameters[0], parameter);
    var rightVisitor = new ReplaceParameterVisitor(expr2.Parameters[0], parameter);
    var left = leftVisitor.Visit(expr1.Body);
    var right = rightVisitor.Visit(expr2.Body);
    var combined = Expression.AndAlso(left!, right!);
    return Expression.Lambda<Func<T, bool>>(combined, parameter);
  }

  ISpecification<Client> ISpecification<Client>.AddInclude(Expression<Func<Client, object>> include)
  {
    return AddInclude(include);
  }

  ISpecification<Client> ISpecification<Client>.AddFilter(Expression<Func<Client, bool>> criteria)
  {
    return AddFilter(criteria);
  }

  ISpecification<Client> ISpecification<Client>.WithPagination(int pageNumber, int pageSize)
  {
    return WithPagination(pageNumber, pageSize);
  }

  private class ReplaceParameterVisitor(ParameterExpression oldParameter,
                                        ParameterExpression newParameter) 
    : ExpressionVisitor
  {
    private readonly ParameterExpression _oldParameter = oldParameter;
    private readonly ParameterExpression _newParameter = newParameter;

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return node == _oldParameter ? _newParameter : base.VisitParameter(node);
    }
  }
}
