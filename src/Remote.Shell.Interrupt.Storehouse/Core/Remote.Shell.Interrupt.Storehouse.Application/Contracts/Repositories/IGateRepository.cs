

namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IGateRepository : IGenericRepository<Gate>
{
  Task<bool> AnyByIPAddressAsync(string iPAddress,
                                 CancellationToken cancellationToken);
  Task<IEnumerable<Gate>> GetAllAsync(RequestParameters requestParameters,
                                      CancellationToken cancellationToken);
}
