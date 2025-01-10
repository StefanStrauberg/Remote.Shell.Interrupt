namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface ISPRVlanRsRepository
{
  Task<IEnumerable<SPRVlanR>> GetAllAsync(CancellationToken cancellationToken);
}
