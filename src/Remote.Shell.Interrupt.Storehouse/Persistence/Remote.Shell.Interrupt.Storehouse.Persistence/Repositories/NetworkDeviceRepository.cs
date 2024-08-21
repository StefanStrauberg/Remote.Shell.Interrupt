namespace Remote.Shell.Interrupt.Storehouse.Persistence.Repositories;

public class NetworkDeviceRepository(IMongoDbSettings settings) : GenericRepository<NetworkDevice>(settings), INetworkDeviceRepository
{
}
