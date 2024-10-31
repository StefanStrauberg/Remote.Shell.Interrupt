namespace Remote.Shell.Interrupt.Storehouse.Persistence.Repositories;

internal class NetworkDeviceRepository(ApplicationDbContext dbContext)
  : GenericRepository<NetworkDevice>(dbContext), INetworkDeviceRepository
{
  async Task<IEnumerable<NetworkDevice>> INetworkDeviceRepository.GetAllWithChildrenAsync(CancellationToken cancellationToken)
    => await _dbSet.AsNoTracking()
                   .ToListAsync(cancellationToken);

  void IGenericRepository<NetworkDevice>.DeleteOne(NetworkDevice entity)
  {
    // Получаем все порты, связанные с устройством
    var ports = entity.PortsOfNetworkDevice
                      .ToList();

    // Удаляем все VLAN, связанные с портами
    var vlanIdsToDelete = ports.SelectMany(port => port.VLANs
                                                       .Select(vlan => vlan))
                               .Distinct()
                               .ToList();

    _dbContext.VLANs
              .RemoveRange(vlanIdsToDelete);

    // Удаляем порты устройства
    _dbContext.Ports
              .RemoveRange(ports);

    // Удаляем само устройство
    _dbSet.Remove(entity);
  }

  async Task<NetworkDevice> IGenericRepository<NetworkDevice>.FirstAsync(Expression<Func<NetworkDevice, bool>> predicate,
                                                                         CancellationToken cancellationToken)
  => await _dbSet.Include(nd => nd.PortsOfNetworkDevice)
                   .ThenInclude(port => port.ARPTableOfInterface)
                 .Include(nd => nd.PortsOfNetworkDevice)
                   .ThenInclude(arpTable => arpTable.NetworkTableOfInterface)
                 .Include(nd => nd.PortsOfNetworkDevice)
                   .ThenInclude(vln => vln.VLANs)
                 .Include(nd => nd.PortsOfNetworkDevice)
                   .ThenInclude(vln => vln.MACTable)
                 .Include(nd => nd.PortsOfNetworkDevice)
                   .ThenInclude(port => port.AggregatedPorts) // Добавлено для загрузки агрегированных портов
                 .FirstAsync(predicate, cancellationToken);

  async Task<NetworkDevice> INetworkDeviceRepository.FindOneWithChildrenAsync(Expression<Func<NetworkDevice, bool>> filterExpression,
                                                                              CancellationToken cancellationToken)
    => await _dbSet.AsNoTracking()
                   .Include(nd => nd.PortsOfNetworkDevice)
                     .ThenInclude(port => port.ARPTableOfInterface)
                   .Include(nd => nd.PortsOfNetworkDevice)
                     .ThenInclude(arpTable => arpTable.NetworkTableOfInterface)
                   .Include(nd => nd.PortsOfNetworkDevice)
                     .ThenInclude(vln => vln.VLANs)
                   .Include(nd => nd.PortsOfNetworkDevice)
                     .ThenInclude(vln => vln.MACTable)
                   .Include(nd => nd.PortsOfNetworkDevice)
                     .ThenInclude(port => port.AggregatedPorts) // Добавлено для загрузки агрегированных портов
                   .FirstAsync(filterExpression, cancellationToken);

  async Task<IEnumerable<NetworkDevice>> INetworkDeviceRepository.FindManyWithChildrenAsync(Expression<Func<NetworkDevice, bool>> filterExpression,
                                                                                            CancellationToken cancellationToken)
    => await _dbSet.AsNoTracking()
                   .Include(nd => nd.PortsOfNetworkDevice)
                     .ThenInclude(port => port.ARPTableOfInterface)
                   .Include(nd => nd.PortsOfNetworkDevice)
                     .ThenInclude(arpTable => arpTable.NetworkTableOfInterface)
                    .Include(nd => nd.PortsOfNetworkDevice)
                      .ThenInclude(vln => vln.VLANs)
                   .Include(nd => nd.PortsOfNetworkDevice)
                     .ThenInclude(vln => vln.MACTable)
                   .Include(nd => nd.PortsOfNetworkDevice)
                     .ThenInclude(port => port.AggregatedPorts) // Добавлено для загрузки агрегированных портов
                   .Where(predicate: filterExpression)
                   .ToListAsync(cancellationToken: cancellationToken);
}
