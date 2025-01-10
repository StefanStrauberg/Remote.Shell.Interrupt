namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IClientCODRRepository
{
  Task<IEnumerable<ClientCODR>> GetAllAsync(CancellationToken cancellationToken);
}
