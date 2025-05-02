namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

/// <summary>
/// Provides an interface for deleting a single entity.
/// </summary>
/// <typeparam name="T">The type of entity that implements <see cref="BaseEntity"/>.</typeparam>
public interface IDeleteRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Deletes a single entity.
    /// </summary>
    /// <param name="entity">The entity to be deleted.</param>
    void DeleteOne(T entity);
}
