namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

public interface IExistenceQueryRepository<T> where T : BaseEntity
{
    Task<bool> AnyByQueryAsync(RequestParameters requestParameters,
                               CancellationToken cancellationToken);
}