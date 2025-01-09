namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IClientCODRepository
{
  Task<IEnumerable<ClientCodR>> GetAllAsync(CancellationToken cancellationToken);
  Task<IEnumerable<ClientCodR>> GetAllByNameAsync(string name,
                                                  CancellationToken cancellationToken);
  Task<string?> GetClientNameByVlanTagAsync(int tag,
                                            CancellationToken cancellationToken);
}
