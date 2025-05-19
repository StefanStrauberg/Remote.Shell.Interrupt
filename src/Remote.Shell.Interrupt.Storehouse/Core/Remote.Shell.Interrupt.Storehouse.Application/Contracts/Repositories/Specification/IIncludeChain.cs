namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.Specification;

public interface IIncludeChain<TBase> 
  where TBase : BaseEntity
{
  IReadOnlyList<(Type EntityType, Type PropertyType, LambdaExpression Expression)> Includes { get; }
  IIncludeChain<TBase> AddInclude<TProperty>(Expression<Func<TBase, TProperty>> include);
  IIncludeChain<TBase> AddThenInclude<TPreviousProperty, TProperty>(Expression<Func<TPreviousProperty, TProperty>> thenInclude);
}
