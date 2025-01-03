namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface ITfPlanRRepository
{
  Task<IEnumerable<TfPlanR>> GetAllAsync(CancellationToken cancellationToken);
}