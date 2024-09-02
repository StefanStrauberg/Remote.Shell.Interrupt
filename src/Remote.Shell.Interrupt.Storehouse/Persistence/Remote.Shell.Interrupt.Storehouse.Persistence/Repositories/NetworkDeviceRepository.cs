namespace Remote.Shell.Interrupt.Storehouse.Persistence.Repositories;

internal class NetworkDeviceRepository(IDocumentSession session)
  : GenericRepository<NetworkDevice>(session), INetworkDeviceRepository
{
}
