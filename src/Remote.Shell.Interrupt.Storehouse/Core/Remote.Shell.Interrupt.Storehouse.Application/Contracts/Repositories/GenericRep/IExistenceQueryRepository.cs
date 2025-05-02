namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

/// <summary>
/// Provides an interface for checking the existence of entities based on a query specification.
/// </summary>
/// <typeparam name="T">The type of entity that implements <see cref="BaseEntity"/>.</typeparam>
public interface IExistenceQueryRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Asynchronously determines whether any entities match the given specification.
    /// </summary>
    /// <param name="specification">The specification used to filter the entities.</param>
    /// <param name="cancellationToken">A token for canceling the operation if needed.</param>
    /// <returns><c>true</c> if at least one entity matches the specification; otherwise, <c>false</c>.</returns>
    Task<bool> AnyByQueryAsync(ISpecification<T> specification,
                               CancellationToken cancellationToken);
}