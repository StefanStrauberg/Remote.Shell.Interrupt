namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

public interface IBulkDeleteRepository<T> where T : BaseEntity
{
    void DeleteMany(IEnumerable<T> entities);
}