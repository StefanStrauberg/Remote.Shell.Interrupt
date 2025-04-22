namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

public interface IWriteManyRepository<T> where T : BaseEntity
{
    void InsertMany(IEnumerable<T> entities);

    void ReplaceMany(IEnumerable<T> entities);

    void DeleteMany(IEnumerable<T> entities);
}
