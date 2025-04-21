namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IClientsRepository : IGenericRepository<Client>
{
  Task<IEnumerable<Client>> GetManyWithChildrensByQueryAsync(RequestParameters requestParameters,
                                                             CancellationToken cancellationToken);
  Task<Client> GetOneWithChildrensByQueryAsync(RequestParameters requestParameters,
                                               CancellationToken cancellationToken);
  Task<IEnumerable<Client>> GetShortManyByQueryAsync(RequestParameters requestParameters,
                                                     CancellationToken cancellationToken);
}
