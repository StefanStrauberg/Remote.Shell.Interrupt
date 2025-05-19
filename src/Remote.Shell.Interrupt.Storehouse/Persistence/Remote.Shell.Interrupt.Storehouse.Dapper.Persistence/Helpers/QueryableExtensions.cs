namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Helpers;

internal static class QueryableExtensions
{
  /// <summary>
  /// Applies multiple include expressions to the query.
  /// </summary>
  /// <typeparam name="T">The entity type being queried.</typeparam>
  /// <param name="query">The base query.</param>
  /// <param name="includes">Collection of expressions specifying navigation properties to include.</param>
  /// <returns>An <see cref="IQueryable{T}"/> with the applied includes.</returns>
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
          // Применяем Include
          var includeMethod = typeof(EntityFrameworkQueryableExtensions).GetMethods()
                                                                        .First(x => x.Name == nameof(EntityFrameworkQueryableExtensions.Include) &&
                                                                               x.GetParameters().Length == 2)
                                                                        .MakeGenericMethod(typeof(T), propertyType);
          query = (IQueryable<T>)includeMethod.Invoke(null, [query, expression])!;
        }
        else
        {
          // Применяем ThenInclude
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

  public static IQueryable<T> ApplyTake<T>(this IQueryable<T> query, int take)
    => take > 0 ? query.Take(take) : query;

  public static IQueryable<T> ApplySkip<T>(this IQueryable<T> query, int skip)
    => skip > 0 ? query.Skip(skip) : query;

  public static IQueryable<T> ApplyWhere<T>(this IQueryable<T> query, Expression<Func<T, bool>>? criterias)
    => criterias is not null ? query.Where(criterias) : query;
}
