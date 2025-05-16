namespace Remote.Shell.Interrupt.Storehouse.Specification.Specifications;

public class IncludeChain<TBase> : IIncludeChain<TBase>  
  where TBase : BaseEntity
{
  readonly List<LambdaExpression> _includes;

  public IncludeChain()
    => _includes = [];
  
  public IncludeChain(IEnumerable<LambdaExpression> includes)
    => _includes = [.. includes];

  public IncludeChain(Expression<Func<TBase>> include)
    => _includes!.Add(include);

  public IncludeChain<TBase> AddInclude<TProperty>(Expression<Func<TBase, TProperty>> include)
  {
    _includes.Add(include);
    return this;
  }

  public IncludeChain<TBase> AddThenInclude<TProperty, TNextProperty>(Expression<Func<TProperty, TNextProperty>> thenInclude)
  {
    _includes.Add(thenInclude);
    return this;
  }

  public IReadOnlyList<LambdaExpression> Includes 
    => _includes;
}
