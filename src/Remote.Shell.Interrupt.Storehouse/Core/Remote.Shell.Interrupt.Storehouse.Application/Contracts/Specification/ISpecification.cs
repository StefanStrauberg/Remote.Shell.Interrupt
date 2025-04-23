namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Specification;

public interface ISpecification<T> where T : BaseEntity
{
  Expression<Func<T, bool>>? Criteria { get; }
  IReadOnlyList<Expression<Func<T, object>>> Includes { get; }
  int Take { get; }
  int Skip { get; }

  ISpecification<T> AddInclude(Expression<Func<T, object>> include);
  ISpecification<T> AddFilter(Expression<Func<T, bool>> criteria);
  ISpecification<T> WithPagination(int pageNumber, int pageSize);
}
