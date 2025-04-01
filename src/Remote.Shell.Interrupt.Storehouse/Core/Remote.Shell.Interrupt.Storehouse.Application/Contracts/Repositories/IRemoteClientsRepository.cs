namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IRemoteClientsRepository
{
  Task<IEnumerable<RemoteClient>> GetAllAsync(CancellationToken cancellationToken);
}
