namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

/// <summary>
/// Provides an interface for bulk replacement operations on entities.
/// </summary>
/// <typeparam name="T">The type of entity that implements <see cref="BaseEntity"/>.</typeparam>
public interface IBulkReplaceRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Replaces multiple entities in a single operation.
    /// </summary>
    /// <param name="entities">The collection of entities to be replaced.</param>
    void ReplaceMany(IEnumerable<T> entities);
}
