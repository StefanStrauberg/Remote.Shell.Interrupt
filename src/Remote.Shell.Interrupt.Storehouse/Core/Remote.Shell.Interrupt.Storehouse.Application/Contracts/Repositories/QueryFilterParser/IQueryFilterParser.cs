namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.QueryFilterParser;

/// <summary>
/// Defines a parser for creating filtering expressions based on a list of filter descriptors.
/// </summary>
public interface IQueryFilterParser
{
  /// <summary>
  /// Parses a list of filter descriptors into a LINQ expression for filtering entities.
  /// </summary>
  /// <typeparam name="T">The type of entity to which the filter applies.</typeparam>
  /// <param name="filters">A list of <see cref="FilterDescriptor"/> objects representing the filters to parse.</param>
  /// <returns>
  /// A LINQ expression that represents the combined filtering criteria, 
  /// or <c>null</c> if the filter list is empty or invalid.
  /// </returns>
  Expression<Func<T, bool>>? ParseFilters<T>(List<FilterDescriptor>? filters);

/// <summary>
  /// Parses a property name into a LINQ expression for sorting entities.
  /// </summary>
  /// <typeparam name="T">The type of entity to sort.</typeparam>
  /// <param name="propertyName">The name of the property to sort by.</param>
  /// <returns>
  /// A LINQ expression representing the property accessor for sorting,
  /// or <c>null</c> if the property name is null or invalid.
  /// </returns>
  Expression<Func<T, Object>>? ParseOrderBy<T>(string? propertyName);
}
