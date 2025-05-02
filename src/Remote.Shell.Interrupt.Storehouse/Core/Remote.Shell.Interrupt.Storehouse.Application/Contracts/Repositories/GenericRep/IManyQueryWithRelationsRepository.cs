namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

/// <summary>
/// Provides an interface for querying multiple entities along with their related child entities.
/// </summary>
/// <typeparam name="T">The type of entity that implements <see cref="BaseEntity"/>.</typeparam>
public interface IManyQueryWithRelationsRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Asynchronously retrieves multiple entities along with their associated child entities
    /// that satisfy the given specification.
    /// </summary>
    /// <param name="specification">The specification used to filter the entities.</param>
    /// <param name="cancellationToken">A token for canceling the operation if needed.</param>
    /// <returns>An enumerable collection of entities including their related child data.</returns>
    Task<IEnumerable<T>> GetManyWithChildrenAsync(ISpecification<T> specification,
                                                  CancellationToken cancellationToken);
}
