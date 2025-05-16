namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Helpers;

public static class QueryableExtensions
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
    foreach (var chain in includeChains.OfType<IIncludeChain<T>>())
    {
      var includes = chain.Includes;
      IIncludableQueryable<T, object>? currentInclude = null;
      
      foreach (var include in includes)
      {
        if (currentInclude == null)
          currentInclude = query.Include((Expression<Func<T, object>>)include);
        else
          currentInclude = currentInclude.ThenInclude((Expression<Func<object, object>>)include);
      }
      
      if (currentInclude != null)
        query = currentInclude as IQueryable<T> ?? query;
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
