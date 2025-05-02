namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

/// <summary>
/// Provides an interface for counting entities that match a given specification.
/// </summary>
/// <typeparam name="T">The type of entity that implements <see cref="BaseEntity"/>.</typeparam>
public interface ICountRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Asynchronously retrieves the count of entities matching the specified criteria.
    /// </summary>
    /// <param name="specification">The specification used to filter the entities.</param>
    /// <param name="cancellationToken">A token for canceling the operation if needed.</param>
    /// <returns>The number of entities that satisfy the given specification.</returns>
    Task<int> GetCountAsync(ISpecification<T> specification,
                            CancellationToken cancellationToken);
}