namespace Remote.Shell.Interrupt.Storehouse.Application.Models.Response;

/// <summary>
/// Provides a utility method for generating empty paginated result sets.
/// </summary>
internal static class EmptyResult
{
  /// <summary>
  /// Returns an empty paginated list of the specified DTO type.
  /// </summary>
  /// <typeparam name="TResult">The type of the DTO to wrap in the paginated result.</typeparam>
  /// <returns>
  /// A <see cref="PagedList{T}"/> with an empty result set, zero total count,
  /// and default pagination values set to 0.
  /// </returns>
  public static PagedList<TResult> GetFor<TResult>()
    => new([], 0, new(0, 0));
}