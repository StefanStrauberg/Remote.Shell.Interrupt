
namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface ISPRVlansRepository : IGenericRepository<SPRVlan>
{
  Task<bool> AnyByVlanTagAsync(int vlanTag,
                               CancellationToken cancellationToken);
  Task<IEnumerable<SPRVlan>> GetSPRVlansByQueryAsync(RequestParameters requestParameters,
                                                     CancellationToken cancellationToken);
  Task<SPRVlan> GetSPRVlanByQueryAsync(RequestParameters requestParameters,
                                       CancellationToken cancellationToken);
}