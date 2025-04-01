namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IRemoteSPRVlansRepository
{
  Task<IEnumerable<RemoteSPRVlan>> GetAllAsync(CancellationToken cancellationToken);
}
