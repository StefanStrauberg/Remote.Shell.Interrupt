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
                                               IReadOnlyCollection<Expression<Func<T, object>>> includes)
    where T : BaseEntity
  {
    if (includes is null || includes.Count == 0)
      return query;

    foreach (var include in includes)
      query = query.Include(include);

    return query;
  }

  public static IQueryable<T> ApplyTake<T>(this IQueryable<T> query, int take)
  {
    if (take > 0) 
      return query.Take(take); 
      
    return query;
  }

  public static IQueryable<T> ApplySkip<T>(this IQueryable<T> query, int skip)
  {
    if (skip > 0) 
      return query.Skip(skip); 
      
    return query;
  }

  public static IQueryable<T> ApplyWhere<T>(this IQueryable<T> query, Expression<Func<T, bool>>? criterias)
  {
    if (criterias is null) 
      return query; 
      
    return query.Where(criterias);
  }
}
