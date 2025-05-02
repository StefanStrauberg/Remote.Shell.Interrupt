namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

/// <summary>
/// Provides an interface for querying a single entity along with its related child entities.
/// </summary>
/// <typeparam name="T">The type of entity that implements <see cref="BaseEntity"/>.</typeparam>
public interface IOneQueryWithRelationsRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Asynchronously retrieves a single entity along with its associated child entities
    /// that satisfy the given specification.
    /// </summary>
    /// <param name="specification">The specification used to filter the entity.</param>
    /// <param name="cancellationToken">A token for canceling the operation if needed.</param>
    /// <returns>The entity including its related child data.</returns>
    Task<T> GetOneWithChildrenAsync(ISpecification<T> specification,
                                    CancellationToken cancellationToken);
}