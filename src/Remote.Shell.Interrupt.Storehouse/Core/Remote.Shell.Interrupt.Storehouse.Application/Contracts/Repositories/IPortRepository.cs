


namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IPortRepository : IGenericRepository<Port>
{
  Task<IEnumerable<Port>> GetAllAggregatedPortsByIdAsync(Guid id,
                                                         CancellationToken cancellationToken);
  Task<IEnumerable<Port>> GetAllAggregatedPortsByListAsync(List<Guid> Ids,
                                                           CancellationToken cancellationToken);
  Task<string> LookingForInterfaceNameByIPAsync(string ipAddress,
                                                CancellationToken cancellationToken);
}