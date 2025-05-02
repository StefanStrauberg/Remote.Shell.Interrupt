namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

/// <summary>
/// Provides an interface for querying a single entity that matches a given specification.
/// </summary>
/// <typeparam name="T">The type of entity that implements <see cref="BaseEntity"/>.</typeparam>
public interface IOneQueryRepository<T> where T : BaseEntity
{   
    /// <summary>
    /// Asynchronously retrieves a single entity that satisfies the given specification.
    /// </summary>
    /// <param name="specification">The specification used to filter the entity.</param>
    /// <param name="cancellationToken">A token for canceling the operation if needed.</param>
    /// <returns>The entity that matches the specification.</returns>
    Task<T> GetOneShortAsync(ISpecification<T> specification,
                             CancellationToken cancellationToken);
}