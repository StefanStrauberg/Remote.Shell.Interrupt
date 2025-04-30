namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.Specification;

/// <summary>
/// Defines a specification pattern interface for querying entities.
/// </summary>
/// <typeparam name="T">The type of entity that this specification applies to.</typeparam>
public interface ISpecification<T> : ICloneable where T : BaseEntity
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
  IReadOnlyCollection<Expression<Func<T, object>>> Includes { get; }

  /// <summary>
  /// Gets the maximum number of entities to retrieve.
  /// </summary>
  int Take { get; }

  /// <summary>
  /// Gets the number of entities to skip in the query.
  /// </summary>
  int Skip { get; }

  /// <summary>
  /// Adds an include expression to the specification for including related entities.
  /// </summary>
  /// <param name="include">The expression specifying the navigation property to include.</param>
  /// <returns>An updated specification with the include added.</returns>
  ISpecification<T> AddInclude(Expression<Func<T, object>> include);

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
}
