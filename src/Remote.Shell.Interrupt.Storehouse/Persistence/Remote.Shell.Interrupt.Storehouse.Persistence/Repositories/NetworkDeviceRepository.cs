namespace Remote.Shell.Interrupt.Storehouse.Persistence.Repositories;

internal class NetworkDeviceRepository(ApplicationDbContext dbContext)
  : GenericRepository<NetworkDevice>(dbContext), INetworkDeviceRepository
{
  public async Task<IEnumerable<NetworkDevice>> GetAllWithChildrenAsync(CancellationToken cancellationToken)
    => await _dbSet
            .AsNoTracking()
            .Include(nd => nd.PortsOfNetworkDevice)
              .ThenInclude(port => port.ARPTableOfPort)
            .Include(nd => nd.PortsOfNetworkDevice)
              .ThenInclude(arpTable => arpTable.NetworkTableOfPort)
            .Include(nd => nd.PortsOfNetworkDevice)
              .ThenInclude(vln => vln.VLAN)
            .ToListAsync(cancellationToken);
}
