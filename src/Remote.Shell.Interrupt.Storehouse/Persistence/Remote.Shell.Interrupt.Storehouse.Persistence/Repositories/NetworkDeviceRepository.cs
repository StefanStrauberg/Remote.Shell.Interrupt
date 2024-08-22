namespace Remote.Shell.Interrupt.Storehouse.Persistence.Repositories;

internal class NetworkDeviceRepository(IMongoDbSettings settings)
  : GenericRepository<NetworkDevice>(settings), INetworkDeviceRepository
{
}
