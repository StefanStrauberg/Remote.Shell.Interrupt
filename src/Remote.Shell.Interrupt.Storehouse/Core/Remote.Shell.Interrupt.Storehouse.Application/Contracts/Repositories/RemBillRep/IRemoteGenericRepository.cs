namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.RemBillRep;

/// <summary>
/// Defines a generic repository interface for retrieving all entities of a specified type remotely.
/// </summary>
/// <typeparam name="T">The type of entity being queried, constrained to reference types.</typeparam>
public interface IRemoteGenericRepository<T> where T : class
{
  /// <summary>
  /// Asynchronously retrieves all entities of type T from a remote source.
  /// </summary>
  /// <param name="cancellationToken">A token for canceling the operation if needed.</param>
  /// <returns>An enumerable collection of all entities.</returns>
  Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
}
