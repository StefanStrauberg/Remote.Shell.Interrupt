namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

public interface IReplaceRepository<T> where T : BaseEntity
{
    void ReplaceOne(T entity);
}
