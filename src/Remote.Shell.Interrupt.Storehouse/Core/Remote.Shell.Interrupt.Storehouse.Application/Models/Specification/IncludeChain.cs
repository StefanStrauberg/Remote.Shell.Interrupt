namespace Remote.Shell.Interrupt.Storehouse.Application.Models.Specification;

public class IncludeChain<TBase, TProperty> : IIncludeChain<TBase>  where TBase : BaseEntity
{
  readonly List<LambdaExpression> _includes = [];
  
  public IncludeChain(Expression<Func<TBase, TProperty>> include)
    => _includes.Add(include);

  public IncludeChain<TBase, TNextProperty> AddThenInclude<TNextProperty>(Expression<Func<TProperty, TNextProperty>> thenInclude)
  {
     _includes.Add(thenInclude);
    return new IncludeChain<TBase, TNextProperty>(_includes);
  }

  private IncludeChain(List<LambdaExpression> existingIncludes)
  {
    _includes = existingIncludes;
  }

  public IReadOnlyList<LambdaExpression> Includes 
    => _includes;
}
