namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.Specification;

/// <summary>
/// Defines a specification pattern interface for querying entities.
/// </summary>
/// <typeparam name="TBase">The type of entity that this specification applies to.</typeparam>
public interface ISpecification<TBase> where TBase : BaseEntity
{ 
  /// <summary>
  /// Gets the criteria expression used for filtering entities.
  /// </summary>
  /// <remarks>
  /// This is a lambda expression defining the filter condition.
  /// </remarks>
  Expression<Func<TBase, bool>>? Criterias { get; }

  /// <summary>
  /// Gets the expression used to sort entities in ascending order.
  /// </summary>
  Expression<Func<TBase, object>>? OrderBy { get; }

  /// <summary>
  /// Gets the expression used to sort entities in descending order.
  /// </summary>
  Expression<Func<TBase, object>>? OrderByDescending { get; }

  /// <summary>
  /// Gets the list of expressions used to include related entities.
  /// </summary>
  /// <remarks>
  /// These expressions specify navigation properties to include in the query.
  /// </remarks>
  IReadOnlyList<IIncludeChain<TBase>> IncludeChains { get; }

  /// <summary>
  /// Gets the maximum number of entities to retrieve.
  /// </summary>
  int Take { get; }

  /// <summary>
  /// Gets the number of entities to skip in the query.
  /// </summary>
  int Skip { get; }

  /// <summary>
  /// Adds an include expression for eager loading of related entities.
  /// </summary>
  /// <typeparam name="TProperty">The type of the related property.</typeparam>
  /// <param name="include">The navigation property to include.</param>
  /// <returns>The updated specification with the include applied.</returns>
  ISpecification<TBase> AddInclude<TProperty>(Expression<Func<TBase, TProperty>> include);

  /// <summary>
  /// Adds a nested include expression for eager loading of related entities.
  /// </summary>
  /// <typeparam name="TPrevious">The type of the previously included entity.</typeparam>
  /// <typeparam name="TProperty">The type of the nested property to include.</typeparam>
  /// <param name="thenInclude">The nested navigation property to include.</param>
  /// <returns>The updated specification with the nested include applied.</returns>
  ISpecification<TBase> AddThenInclude<TPrevious, TProperty>(Expression<Func<TPrevious, TProperty>> thenInclude);

  /// <summary>
  /// Adds a filtering criterion to the specification.
  /// </summary>
  /// <param name="criteria">The filtering expression to add.</param>
  /// <returns>An updated specification with the filter added.</returns>  
  ISpecification<TBase> AddFilter(Expression<Func<TBase, bool>> criteria);

  /// <summary>
  /// Adds an ascending sort expression to the specification.
  /// </summary>
  /// <typeparam name="TKey">The type of the key to sort by.</typeparam>
  /// <param name="orderByExpression">The expression specifying the sort key.</param>
  /// <returns>The updated specification with the sort applied.</returns>
  ISpecification<TBase> AddOrderBy<TKey>(Expression<Func<TBase, TKey>> orderByExpression);

  /// <summary>
  /// Adds a descending sort expression to the specification.
  /// </summary>
  /// <typeparam name="TKey">The type of the key to sort by.</typeparam>
  /// <param name="orderByExpression">The expression specifying the sort key.</param>
  /// <returns>The updated specification with the sort applied.</returns>
  ISpecification<TBase> AddOrderByDescending<TKey>(Expression<Func<TBase, TKey>> orderByExpression);

  /// <summary>
  /// Configures the specification with pagination parameters.
  /// </summary>
  /// <param name="paginationContext">The page size and number to retrieve.</param>
  /// <returns>An updated specification with pagination settings.</returns>
  ISpecification<TBase> ConfigurePagination(PaginationContext paginationContext);

  /// <summary>
  /// Creates a deep copy of the current specification instance.
  /// </summary>
  /// <returns>A new specification instance with the same criteria, includes, and pagination settings.</returns>
  /// <remarks>
  /// This method allows creating an independent copy of the current specification,
  /// ensuring that modifications to the new instance do not affect the original.
  /// </remarks>
  ISpecification<TBase> Clone();
}
