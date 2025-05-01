namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

public interface IManyQueryWithRelationsRepository<T> where T : BaseEntity
{
    Task<IEnumerable<T>> GetManyWithChildrenAsync(ISpecification<T> specification,
                                                  CancellationToken cancellationToken);
}
