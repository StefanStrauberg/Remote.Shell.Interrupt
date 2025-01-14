namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IClientCODLRepository : IGenericRepository<ClientCODL>
{
  Task<IEnumerable<ClientCODL>> GetAllByNameAsync(string name,
                                                  CancellationToken cancellationToken);
  Task<IEnumerable<ClientCODL>> GetAllByNameWithChildrensAsync(string name,
                                                               CancellationToken cancellationToken);
  Task<string?> GetClientNameByClientIdAsync(int clientId,
                                             CancellationToken cancellationToken);

  Task<IEnumerable<ClientCODL>> GetAllWithChildrensAsync(CancellationToken cancellationToken);
}
