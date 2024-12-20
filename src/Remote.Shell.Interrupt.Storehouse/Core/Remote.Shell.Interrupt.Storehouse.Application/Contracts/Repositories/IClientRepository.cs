namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IClientRepository
{
  Task<IEnumerable<ClientCOD>> GetAllAsync(CancellationToken cancellationToken);
  Task<IEnumerable<ClientCOD>> GetAllByNameAsync(string name,
                                                 CancellationToken cancellationToken);
  Task<string?> GetClientNameByVlanTagAsync(int tag,
                                            CancellationToken cancellationToken);
}
