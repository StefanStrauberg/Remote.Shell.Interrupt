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
    Criteria = Criteria == null ? criteria 
                                : Expression.Lambda<Func<Client, bool>>(Expression.AndAlso(Criteria.Body, criteria.Body),
                                                                        Criteria.Parameters[0]);
    return this;
  }

  public ISpecification<Client> AddInclude(Expression<Func<Client, object>> include)
  {
    _includes.Add(include);
    return this;
  }

  public ISpecification<Client> WithPagination(int pageNumber, int pageSize)
  {
    Skip = (pageNumber - 1) * pageSize;
    Take = pageSize;
    return this;
  }
}
