namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

public interface IQueryableWithChildrenRepository<T> where T : BaseEntity
{
    Task<IEnumerable<T>> GetManyWithChildrensByQueryAsync(RequestParameters requestParameters,
                                                          CancellationToken cancellationToken);
    Task<T> GetOneWithChildrensByQueryAsync(RequestParameters requestParameters,
                                            CancellationToken cancellationToken);
}
