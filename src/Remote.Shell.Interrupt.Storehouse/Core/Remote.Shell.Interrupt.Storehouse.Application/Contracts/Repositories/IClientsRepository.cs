namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IClientsRepository : IGenericRepository<Client>
{
  Task<IEnumerable<Client>> GetClientsWithChildrensByQueryAsync(RequestParameters requestParameters,
                                                                CancellationToken cancellationToken);
  Task<IEnumerable<Client>> GetShortClientsByQueryAsync(RequestParameters requestParameters,
                                                        CancellationToken cancellationToken);
}
