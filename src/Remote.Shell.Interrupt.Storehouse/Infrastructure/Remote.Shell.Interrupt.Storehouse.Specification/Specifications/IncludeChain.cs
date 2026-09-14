namespace Remote.Shell.Interrupt.Storehouse.Specification.Specifications;

internal class IncludeChain<TBase> : IIncludeChain<TBase> where TBase : BaseEntity
{
  readonly List<IncludeChainItem> _includes = [];

  public IReadOnlyList<(Type EntityType, Type PropertyType, LambdaExpression Expression)> Includes
    => [.. _includes.Select(x => (x.EntityType, x.PropertyType, x.Expression))];

  public IIncludeChain<TBase> AddInclude<TProperty>(Expression<Func<TBase, TProperty>> include)
  {
    _includes.Add(new IncludeChainItem(typeof(TBase), typeof(TProperty), include));
    return this;
  }

  public IIncludeChain<TBase> AddThenInclude<TPrevious, TProperty>(Expression<Func<TPrevious, TProperty>> thenInclude)
  {
    if (_includes.Count == 0)
      throw new InvalidOperationException("Cannot add ThenInclude without Include");

    _includes.Add(new IncludeChainItem(typeof(TPrevious), typeof(TProperty), thenInclude));
    return this;
  }

  public void AddTypedInclude<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> include)
    => _includes.Add(new IncludeChainItem(typeof(TEntity), typeof(TProperty), include));

  public IncludeChain<TBase> Clone()
  {
    var clone = new IncludeChain<TBase>();
    clone._includes.AddRange(_includes);
    return clone;
  }

  record IncludeChainItem(Type EntityType, Type PropertyType, LambdaExpression Expression);
}
