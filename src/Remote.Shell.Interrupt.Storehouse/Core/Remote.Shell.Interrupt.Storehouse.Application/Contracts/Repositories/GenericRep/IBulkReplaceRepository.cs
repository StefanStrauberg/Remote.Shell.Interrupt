namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

public interface IBulkReplaceRepository<T> where T : BaseEntity
{
    void ReplaceMany(IEnumerable<T> entities);
}
