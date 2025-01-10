namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IClientCodLRepository : IGenericRepository<ClientCODL>
{
  Task<IEnumerable<ClientCODL>> GetAllByNameAsync(string name,
                                                  CancellationToken cancellationToken);
  Task<string?> GetClientNameByVlanTagAsync(int tag,
                                            CancellationToken cancellationToken);
}
