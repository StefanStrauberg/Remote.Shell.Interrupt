
namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface ISPRVlansRepository : IGenericRepository<SPRVlan>
{
  Task<bool> AnyByVlanTagAsync(int vlanTag,
                               CancellationToken cancellationToken);
  Task<IEnumerable<SPRVlan>> GetAllByClientIdsAsync(IEnumerable<int> ids,
                                                    CancellationToken cancellationToken);
  Task<IEnumerable<SPRVlan>> GetAllSPRVlansAsync(RequestParameters requestParameters,
                                                 CancellationToken cancellationToken);
  Task<IEnumerable<int>> GetClientsIdsByVlantTag(int vlanTag,
                                                 CancellationToken cancellationToken);
}