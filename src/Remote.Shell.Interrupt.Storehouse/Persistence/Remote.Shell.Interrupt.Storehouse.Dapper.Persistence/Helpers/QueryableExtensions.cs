namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Helpers;

internal static class QueryableExtensions
{
  /// <summary>
  /// Applies multiple include expressions to the query.
  /// </summary>
  /// <typeparam name="T">The entity type being queried.</typeparam>
  /// <param name="query">The base query.</param>
  /// <param name="includeChains">Collection of include chains specifying navigation properties.</param>
  /// <returns>An <see cref="IQueryable{T}"/> with the applied includes.</returns>
  /// <remarks>
  /// Uses reflection to dynamically invoke <c>Include</c> and <c>ThenInclude</c> methods from EF Core.
  /// Supports nested includes via <see cref="IIncludeChain{T}"/>.
  /// </remarks>
  public static IQueryable<T> ApplyIncludes<T>(this IQueryable<T> query,
                                               IEnumerable<IIncludeChain<T>> includeChains)
    where T : BaseEntity
  {
    foreach (var chain in includeChains)
    {
      foreach (var includeInfo in chain.Includes)
      {
        var (entityType, propertyType, expression) = includeInfo;

        if (entityType == typeof(T))
        {
          var includeMethod = typeof(EntityFrameworkQueryableExtensions).GetMethods()
                                                                        .First(x => x.Name == nameof(EntityFrameworkQueryableExtensions.Include) &&
                                                                               x.GetParameters().Length == 2)
                                                                        .MakeGenericMethod(typeof(T), propertyType);
          query = (IQueryable<T>)includeMethod.Invoke(null, [query, expression])!;
        }
        else
        {
          var genericMethod = typeof(EntityFrameworkQueryableExtensions).GetMethods()
                                                                        .First(m => m.Name == nameof(EntityFrameworkQueryableExtensions.ThenInclude) &&
                                                                               m.GetParameters().Length == 2 &&
                                                                               m.GetParameters()[0].ParameterType.IsGenericType &&
                                                                               m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IIncludableQueryable<,>))
                                                                        .MakeGenericMethod(typeof(T), entityType, propertyType);
          query = (IQueryable<T>)genericMethod.Invoke(null, [query, expression])!;
        }
      }
    }
    return query;
  }

  /// <summary>
  /// Applies a <c>Take</c> clause to the query if the value is greater than zero.
  /// </summary>
  /// <typeparam name="T">The entity type.</typeparam>
  /// <param name="query">The base query.</param>
  /// <param name="take">The number of items to take.</param>
  /// <returns>The modified query with the <c>Take</c> applied.</returns>
  public static IQueryable<T> ApplyTake<T>(this IQueryable<T> query, int take)
    => take > 0 ? query.Take(take) : query;

  /// <summary>
  /// Applies a <c>Skip</c> clause to the query if the value is greater than zero.
  /// </summary>
  /// <typeparam name="T">The entity type.</typeparam>
  /// <param name="query">The base query.</param>
  /// <param name="skip">The number of items to skip.</param>
  /// <returns>The modified query with the <c>Skip</c> applied.</returns>
  public static IQueryable<T> ApplySkip<T>(this IQueryable<T> query, int skip)
    => skip > 0 ? query.Skip(skip) : query;

  /// <summary>
  /// Applies a filtering expression to the query.
  /// </summary>
  /// <typeparam name="T">The entity type.</typeparam>
  /// <param name="query">The base query.</param>
  /// <param name="criterias">The filtering expression.</param>
  /// <returns>The modified query with the filter applied, or the original if null.</returns>
  public static IQueryable<T> ApplyWhere<T>(this IQueryable<T> query, Expression<Func<T, bool>>? criterias)
    => criterias is not null ? query.Where(criterias) : query;

  /// <summary>
  /// Applies sorting to the query based on the provided key selector.
  /// </summary>
  /// <typeparam name="T">The entity type.</typeparam>
  /// <typeparam name="TKey">The type of the sorting key.</typeparam>
  /// <param name="query">The base query.</param>
  /// <param name="keySelector">The expression specifying the sort key.</param>
  /// <param name="descending">Whether to sort in descending order.</param>
  /// <returns>The modified query with sorting applied, or the original if selector is null.</returns>
  public static IQueryable<T> ApplyOrderBy<T, TKey>(this IQueryable<T> query,
                                                    Expression<Func<T, TKey>>? keySelector,
                                                    bool descending = false)
  {
    if (keySelector is null)
      return query;

    return descending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
  }
}
