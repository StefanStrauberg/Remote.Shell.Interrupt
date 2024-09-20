namespace Remote.Shell.Interrupt.Storehouse.Persistence.Repositories;

internal class NetworkDeviceRepository(ApplicationDbContext dbContext)
  : GenericRepository<NetworkDevice>(dbContext), INetworkDeviceRepository
{
  public async Task<IEnumerable<NetworkDevice>> GetAllWithChildrenAsync(CancellationToken cancellationToken)
    => await _dbSet.AsNoTracking()
                   .Include(nd => nd.PortsOfNetworkDevice)
                     .ThenInclude(port => port.ARPTableOfInterface)
                   .Include(nd => nd.PortsOfNetworkDevice)
                     .ThenInclude(arpTable => arpTable.NetworkTableOfInterface)
                   .Include(nd => nd.PortsOfNetworkDevice)
                     .ThenInclude(vln => vln.VLANs)
                   .Include(nd => nd.PortsOfNetworkDevice)
                     .ThenInclude(port => port.AggregatedPorts) // Добавлено для загрузки агрегированных портов
                   .ToListAsync(cancellationToken);

  public override async Task DeleteOneAsync(NetworkDevice document, CancellationToken cancellationToken)
  {
    // Удалите NetworkDevice
    _dbSet.Remove(document);

    // Удалите все VLAN, которые не связаны с другими Port
    var vlanIdsToDelete = document.PortsOfNetworkDevice
                                  .SelectMany(p => p.VLANs.Select(v => v.Id))
                                  .Distinct();

    var unusedVlans = await _dbContext.VLANs
                                      .Where(v => !v.Ports.Any(p => vlanIdsToDelete.Contains(v.Id)))
                                      .ToListAsync(cancellationToken);

    _dbContext.VLANs.RemoveRange(unusedVlans);

    await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task<NetworkDevice> FindOneWithChildrenAsync(Expression<Func<NetworkDevice, bool>> filterExpression,
                                                            CancellationToken cancellationToken)
    => await _dbSet.AsNoTracking()
                   .Include(nd => nd.PortsOfNetworkDevice)
                     .ThenInclude(port => port.ARPTableOfInterface)
                   .Include(nd => nd.PortsOfNetworkDevice)
                     .ThenInclude(arpTable => arpTable.NetworkTableOfInterface)
                   .Include(nd => nd.PortsOfNetworkDevice)
                     .ThenInclude(vln => vln.VLANs)
                   .Include(nd => nd.PortsOfNetworkDevice)
                     .ThenInclude(port => port.AggregatedPorts) // Добавлено для загрузки агрегированных портов
                   .FirstAsync(predicate: filterExpression,
                               cancellationToken: cancellationToken);

  public async Task<IEnumerable<NetworkDevice>> FindManyWithChildrenAsync(Expression<Func<NetworkDevice, bool>> filterExpression,
                                                                          CancellationToken cancellationToken)
    => await _dbSet.AsNoTracking()
                   .Include(nd => nd.PortsOfNetworkDevice)
                     .ThenInclude(port => port.ARPTableOfInterface)
                   .Include(nd => nd.PortsOfNetworkDevice)
                     .ThenInclude(arpTable => arpTable.NetworkTableOfInterface)
                   .Include(nd => nd.PortsOfNetworkDevice)
                     .ThenInclude(vln => vln.VLANs)
                   .Include(nd => nd.PortsOfNetworkDevice)
                     .ThenInclude(port => port.AggregatedPorts) // Добавлено для загрузки агрегированных портов
                   .Where(predicate: filterExpression)
                   .ToListAsync(cancellationToken: cancellationToken);
}
