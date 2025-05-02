namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

/// <summary>
/// Provides an interface for replacing a single entity.
/// </summary>
/// <typeparam name="T">The type of entity that implements <see cref="BaseEntity"/>.</typeparam>
public interface IReplaceRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Replaces a single entity.
    /// </summary>
    /// <param name="entity">The entity to be replaced.</param>
    void ReplaceOne(T entity);
}
