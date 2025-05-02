namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

/// <summary>
/// Provides an interface for bulk insertion operations on entities.
/// </summary>
/// <typeparam name="T">The type of entity that implements <see cref="BaseEntity"/>.</typeparam>
public interface IBulkInsertRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Inserts multiple entities in a single operation.
    /// </summary>
    /// <param name="entities">The collection of entities to be inserted.</param>
    void InsertMany(IEnumerable<T> entities);
}
