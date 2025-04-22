namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

public interface ICountRepository<T> where T : BaseEntity
{
    Task<int> GetCountAsync(RequestParameters requestParameters,
                            CancellationToken cancellationToken);
}