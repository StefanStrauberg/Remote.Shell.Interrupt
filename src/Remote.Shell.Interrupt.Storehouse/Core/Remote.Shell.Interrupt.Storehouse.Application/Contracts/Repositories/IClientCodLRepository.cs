namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IClientCODLRepository : IGenericRepository<ClientCODL>
{
  Task<IEnumerable<ClientCODL>> GetAllByNamesAsync(IEnumerable<string> names,
                                                   CancellationToken cancellationToken);
  Task<IEnumerable<ClientCODL>> GetAllByNameAsync(string name,
                                                  CancellationToken cancellationToken);
  Task<IEnumerable<ClientCODL>> GetAllByNameWithChildrensAsync(string name,
                                                               CancellationToken cancellationToken);
  Task<IEnumerable<string>> GetClientsNamesByClientIdsAsync(IEnumerable<int> clientId,
                                                            CancellationToken cancellationToken);

  Task<IEnumerable<ClientCODL>> GetAllWithChildrensAsync(CancellationToken cancellationToken);
}
