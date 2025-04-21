
namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface ISPRVlansRepository : IGenericRepository<SPRVlan>
{
  Task<bool> AnyByVlanTagAsync(int vlanTag,
                               CancellationToken cancellationToken);
  Task<IEnumerable<SPRVlan>> GetManyByQueryAsync(RequestParameters requestParameters,
                                                 CancellationToken cancellationToken);
}