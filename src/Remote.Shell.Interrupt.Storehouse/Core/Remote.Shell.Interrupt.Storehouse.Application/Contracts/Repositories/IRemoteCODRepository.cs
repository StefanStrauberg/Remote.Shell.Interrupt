namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IRemoteCODRepository
{
  Task<IEnumerable<RemoteCOD>> GetAllAsync(CancellationToken cancellationToken);
}
