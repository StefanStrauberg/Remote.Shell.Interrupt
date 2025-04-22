namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

public interface IDeleteRepository<T> where T : BaseEntity
{
    void DeleteOne(T entity);
}
