namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface INetworkDeviceRepository : IGenericRepository<NetworkDevice>
{
  Task<IEnumerable<NetworkDevice>> GetManyByQueryAsync(RequestParameters requestParameters,
                                                       CancellationToken cancellationToken);
  Task<NetworkDevice> GetOneWithChildrensByIdAsync(Guid id,
                                                   CancellationToken cancellationToken);
  void DeleteOneWithChilren(NetworkDevice networkDeviceToDelete);
  Task<IEnumerable<NetworkDevice>> GetManyWithChildrensByVLANTagAsync(int vlanTag,
                                                                      CancellationToken cancellationToken);
}
