namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

public interface IInsertRepository<T> where T : BaseEntity
{
    void InsertOne(T entity);
}
