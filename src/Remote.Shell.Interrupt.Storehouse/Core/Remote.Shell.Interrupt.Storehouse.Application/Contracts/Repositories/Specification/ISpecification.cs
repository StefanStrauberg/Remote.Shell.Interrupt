namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.Specification;

/// <summary>
/// Defines a specification pattern interface for querying entities.
/// </summary>
/// <typeparam name="T">The type of entity that this specification applies to.</typeparam>
public interface ISpecification<T> where T : BaseEntity
{ 
  /// <summary>
  /// Gets the criteria expression used for filtering entities.
  /// </summary>
  /// <remarks>
  /// This is a lambda expression defining the filter condition.
  /// </remarks>
  Expression<Func<T, bool>>? Criterias { get; }

  /// <summary>
  /// Gets the list of expressions used to include related entities.
  /// </summary>
  /// <remarks>
  /// These expressions specify navigation properties to include in the query.
  /// </remarks>
  IEnumerable<IIncludeChain<T>> IncludeChains { get; }

  /// <summary>
  /// Gets the maximum number of entities to retrieve.
  /// </summary>
  int Take { get; }

  /// <summary>
  /// Gets the number of entities to skip in the query.
  /// </summary>
  int Skip { get; }

  ISpecification<T> AddInclude<TProperty>(Expression<Func<T, TProperty>> include);

  ISpecification<T> AddThenInclude<TPrevious, TProperty>(Expression<Func<TPrevious, TProperty>> thenInclude);

  /// <summary>
  /// Adds a filtering criterion to the specification.
  /// </summary>
  /// <param name="criteria">The filtering expression to add.</param>
  /// <returns>An updated specification with the filter added.</returns>  
  ISpecification<T> AddFilter(Expression<Func<T, bool>> criteria);

  /// <summary>
  /// Configures the specification with pagination parameters.
  /// </summary>
  /// <param name="pageNumber">The page number to retrieve.</param>
  /// <param name="pageSize">The size of the page.</param>
  /// <returns>An updated specification with pagination settings.</returns>
  ISpecification<T> WithPagination(int pageNumber, int pageSize);

  /// <summary>
  /// Creates a deep copy of the current specification instance.
  /// </summary>
  /// <returns>A new specification instance with the same criteria, includes, and pagination settings.</returns>
  /// <remarks>
  /// This method allows creating an independent copy of the current specification,
  /// ensuring that modifications to the new instance do not affect the original.
  /// </remarks>
  ISpecification<T> Clone();
}
