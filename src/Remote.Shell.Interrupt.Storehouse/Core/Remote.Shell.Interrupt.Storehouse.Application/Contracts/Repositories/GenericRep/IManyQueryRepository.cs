namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

/// <summary>
/// Provides an interface for querying multiple entities that match a given specification.
/// </summary>
/// <typeparam name="T">The type of entity that implements <see cref="BaseEntity"/>.</typeparam>
public interface IManyQueryRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Asynchronously retrieves multiple entities that satisfy the given specification.
    /// </summary>
    /// <param name="specification">The specification used to filter the entities.</param>
    /// <param name="cancellationToken">A token for canceling the operation if needed.</param>
    /// <returns>An enumerable collection of entities matching the specification.</returns>
    Task<IEnumerable<T>> GetManyShortAsync(ISpecification<T> specification,
                                           CancellationToken cancellationToken);
}
