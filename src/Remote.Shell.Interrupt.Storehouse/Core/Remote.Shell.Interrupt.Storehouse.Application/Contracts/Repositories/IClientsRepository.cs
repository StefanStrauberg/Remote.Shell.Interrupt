namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IClientsRepository : IGenericRepository<Client>
{
  Task<IEnumerable<Client>> GetAllByNamesAsync(IEnumerable<string> names,
                                                   CancellationToken cancellationToken);
  Task<IEnumerable<Client>> GetAllByNameAsync(string name,
                                              CancellationToken cancellationToken);
  Task<IEnumerable<Client>> GetAllByNameWithChildrensAsync(string name,
                                                           CancellationToken cancellationToken);
  Task<Client> GetClientByIdWithChildrensAsync(Guid id,
                                               CancellationToken cancellationToken);
  Task<IEnumerable<string>> GetClientsNamesByClientIdsAsync(IEnumerable<int> clientId,
                                                            CancellationToken cancellationToken);

  Task<IEnumerable<Client>> GetAllWithChildrensAsync(CancellationToken cancellationToken);
  Task<IEnumerable<Client>> GetAllShortAsync(RequestParameters requestParameters,
                                             CancellationToken cancellationToken);
}
