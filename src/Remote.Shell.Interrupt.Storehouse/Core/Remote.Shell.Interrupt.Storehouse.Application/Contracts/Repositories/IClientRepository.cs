namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IClientCODRepository
{
  Task<IEnumerable<ClientCod>> GetAllAsync(CancellationToken cancellationToken);
  Task<IEnumerable<ClientCod>> GetAllByNameAsync(string name,
                                                 CancellationToken cancellationToken);
  Task<string?> GetClientNameByVlanTagAsync(int tag,
                                            CancellationToken cancellationToken);
}
