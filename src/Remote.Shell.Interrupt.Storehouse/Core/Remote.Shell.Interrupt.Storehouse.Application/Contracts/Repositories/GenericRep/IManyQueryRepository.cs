namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

public interface IManyQueryRepository<T> where T : BaseEntity
{
    Task<IEnumerable<T>> GetManyShortAsync(RequestParameters requestParameters,
                                           CancellationToken cancellationToken,
                                           bool skipFiltering = false);
}
