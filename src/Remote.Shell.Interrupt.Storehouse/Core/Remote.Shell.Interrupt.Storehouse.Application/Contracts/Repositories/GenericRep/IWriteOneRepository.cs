namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

public interface IWriteOneRepository<T> where T : BaseEntity
{
    void InsertOne(T entity);
    void ReplaceOne(T entity);

    void DeleteOne(T entity);
}
