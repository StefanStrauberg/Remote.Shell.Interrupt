namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

public interface IBulkInsertRepository<T> where T : BaseEntity
{
    void InsertMany(IEnumerable<T> entities);
}
