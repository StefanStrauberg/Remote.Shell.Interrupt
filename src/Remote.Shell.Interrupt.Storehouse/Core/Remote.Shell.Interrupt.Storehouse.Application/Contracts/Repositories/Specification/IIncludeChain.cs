namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.Specification;

/// <summary>
/// Defines a contract for constructing an include chain to specify related entities
/// for eager loading in query specifications.
/// </summary>
/// <typeparam name="TBase">The root entity type from which includes are defined.</typeparam>
public interface IIncludeChain<TBase>
  where TBase : BaseEntity
{
  /// <summary>
  /// Gets the ordered collection of include expressions, 
  /// each representing a navigation path to be applied during query execution.
  /// </summary>
  IReadOnlyList<(Type EntityType, Type PropertyType, LambdaExpression Expression)> Includes { get; }

  /// <summary>
  /// Adds a direct navigation property to the include chain.
  /// </summary>
  /// <typeparam name="TProperty">The type of the property to include.</typeparam>
  /// <param name="include">A lambda expression identifying the navigation property from <typeparamref name="TBase"/>.</param>
  /// <returns>The updated <see cref="IIncludeChain{TBase}"/> for chaining.</returns>
  IIncludeChain<TBase> AddInclude<TProperty>(Expression<Func<TBase, TProperty>> include);

  /// <summary>
  /// Adds a nested navigation property to the include chain after the previously added property.
  /// </summary>
  /// <typeparam name="TPreviousProperty">The type of the parent property.</typeparam>
  /// <typeparam name="TProperty">The type of the nested child property to include.</typeparam>
  /// <param name="thenInclude">A lambda expression identifying the child navigation property.</param>
  /// <returns>The updated <see cref="IIncludeChain{TBase}"/> for chaining.</returns>
  IIncludeChain<TBase> AddThenInclude<TPreviousProperty, TProperty>(Expression<Func<TPreviousProperty, TProperty>> thenInclude);
}
