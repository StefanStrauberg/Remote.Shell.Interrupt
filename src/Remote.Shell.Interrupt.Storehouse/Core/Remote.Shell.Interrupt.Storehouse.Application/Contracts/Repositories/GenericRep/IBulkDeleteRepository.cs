namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

/// <summary>
/// Provides an interface for bulk deletion operations on entities.
/// </summary>
/// <typeparam name="T">The type of entity that implements <see cref="BaseEntity"/>.</typeparam>
public interface IBulkDeleteRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Deletes multiple entities in a single operation.
    /// </summary>
    /// <param name="entities">The collection of entities to be deleted.</param>
    void DeleteMany(IEnumerable<T> entities);
}