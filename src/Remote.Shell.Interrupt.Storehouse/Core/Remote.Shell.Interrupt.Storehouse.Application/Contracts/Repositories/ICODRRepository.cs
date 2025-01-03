namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface ICODRRepository
{
  Task<IEnumerable<CODR>> GetAllAsync(CancellationToken cancellationToken);
}
