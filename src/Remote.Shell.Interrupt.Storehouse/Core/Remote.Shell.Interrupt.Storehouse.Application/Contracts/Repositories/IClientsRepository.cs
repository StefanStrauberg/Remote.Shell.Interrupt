namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IClientsRepository : IGenericRepository<Client>
{
  Task<Client> GetClientByIdWithChildrensAsync(Guid id,
                                               CancellationToken cancellationToken);
  Task<Client> GetShortClientByIdAsync(Guid id,
                                       CancellationToken cancellationToken);
  Task<IEnumerable<Client>> GetAllClientsWithChildrensAsync(RequestParameters requestParameters,
                                                            CancellationToken cancellationToken);
  Task<IEnumerable<Client>> GetAllShortClientsAsync(RequestParameters requestParameters,
                                                    CancellationToken cancellationToken);
}
