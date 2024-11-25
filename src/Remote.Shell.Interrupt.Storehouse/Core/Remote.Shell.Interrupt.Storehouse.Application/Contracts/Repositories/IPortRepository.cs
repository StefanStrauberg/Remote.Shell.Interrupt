

namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IPortRepository : IGenericRepository<Port>
{
  Task<string> LookingForInterfaceNameByIP(string ipAddress,
                                           CancellationToken cancellationToken);
}