namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

public interface IOneQueryWithRelationsRepository<T> where T : BaseEntity
{
    Task<T> GetOneWithChildrenAsync(ISpecification<T> specification,
                                    CancellationToken cancellationToken);
}