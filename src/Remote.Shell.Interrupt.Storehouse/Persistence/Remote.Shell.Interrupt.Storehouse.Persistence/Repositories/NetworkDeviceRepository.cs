using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

namespace Remote.Shell.Interrupt.Storehouse.Persistence.Repositories;

internal class NetworkDeviceRepository(ApplicationDbContext dbContext)
  : GenericRepository<NetworkDevice>(dbContext), INetworkDeviceRepository
{
}
