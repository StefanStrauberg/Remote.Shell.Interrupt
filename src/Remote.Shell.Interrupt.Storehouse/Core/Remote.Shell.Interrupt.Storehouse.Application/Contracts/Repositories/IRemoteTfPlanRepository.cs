namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IRemoteTfPlanRepository
{
  Task<IEnumerable<RemoteTfPlan>> GetAllAsync(CancellationToken cancellationToken);
}