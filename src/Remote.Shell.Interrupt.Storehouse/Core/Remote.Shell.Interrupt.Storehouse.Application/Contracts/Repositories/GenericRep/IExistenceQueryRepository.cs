namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

public interface IExistenceQueryRepository<T> where T : BaseEntity
{
    Task<bool> AnyByQueryAsync(ISpecification<T> specification,
                               CancellationToken cancellationToken);
}