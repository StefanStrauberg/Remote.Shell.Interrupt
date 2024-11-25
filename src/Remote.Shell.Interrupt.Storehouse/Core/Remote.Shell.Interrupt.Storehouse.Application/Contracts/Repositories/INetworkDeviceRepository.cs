namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface INetworkDeviceRepository : IGenericRepository<NetworkDevice>
{
  Task<NetworkDevice> GetFirstWithChildrensByIdAsync(Guid id,
                                                     CancellationToken cancellationToken);
  void DeleteOneWithChilren(NetworkDevice networkDeviceToDelete);
  Task<IEnumerable<NetworkDevice>> GetFirstWithChildrensByVLANTagAsync(int vlanTag,
                                                                       CancellationToken cancellationToken);
}
