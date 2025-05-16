namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.Specification;

public interface IIncludeChain<TBase> 
  where TBase : BaseEntity
{
  IReadOnlyList<LambdaExpression> Includes { get; }
}
