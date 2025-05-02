namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

/// <summary>
/// Provides an interface for inserting a single entity.
/// </summary>
/// <typeparam name="T">The type of entity that implements <see cref="BaseEntity"/>.</typeparam>
public interface IInsertRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Inserts a single entity.
    /// </summary>
    /// <param name="entity">The entity to be inserted.</param>
    void InsertOne(T entity);
}
