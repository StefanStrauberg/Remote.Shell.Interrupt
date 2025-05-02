namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

/// <summary>
/// Provides an interface for retrieving all entities of a specified type.
/// </summary>
/// <typeparam name="T">The type of entity that implements <see cref="BaseEntity"/>.</typeparam>
public interface IReadRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Asynchronously retrieves all entities.
    /// </summary>
    /// <param name="cancellationToken">A token for canceling the operation if needed.</param>
    /// <returns>An enumerable collection of all entities.</returns>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
}
