namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.NetDevRep;

internal class NetworkDeviceRepository(ApplicationDbContext context,
                                       IManyQueryRepository<NetworkDevice> manyQueryRepository,
                                       IExistenceQueryRepository<NetworkDevice> existenceQueryRepository,
                                       ICountRepository<NetworkDevice> countRepository,
                                       IInsertRepository<NetworkDevice> insertRepository,
                                       IReadRepository<NetworkDevice> readRepository)
  : INetworkDeviceRepository
{
  void INetworkDeviceRepository.DeleteOneWithChilren(NetworkDevice networkDeviceToDelete)
  {
    // Load Ports with children
    var ports = context.Ports.Include(p => p.ARPTableOfInterface)
                             .Include(p => p.MACTable)
                             .Include(p => p.NetworkTableOfInterface)
                             .Include(p => p.AggregatedPorts)
                             .Include(p => p.VLANs)
                             .Where(p => p.NetworkDeviceId == networkDeviceToDelete.Id)
                             .ToList();

    foreach (var port in ports)
    {
      // Delete ARP entities
      if (port.ARPTableOfInterface.Count > 0)
        context.ARPEntities.RemoveRange(port.ARPTableOfInterface);

      // Delete MAC entities
      if (port.MACTable.Count > 0)
        context.MACEntities.RemoveRange(port.MACTable);

      // Delete terminated networks
      if (port.NetworkTableOfInterface.Count > 0)
        context.TerminatedNetworkEntities.RemoveRange(port.NetworkTableOfInterface);

      // Delete VLANs (not just detach, since they belong only here)
      if (port.VLANs.Count > 0)
        context.VLANs.RemoveRange(port.VLANs);

      // Delete aggregated ports (self-referencing children)
      if (port.AggregatedPorts.Count > 0)
        context.Ports.RemoveRange(port.AggregatedPorts);
    }

    // Delete Ports themselves
    if (ports.Count > 0)
      context.Ports.RemoveRange(ports);

    // Finally delete the device
    context.NetworkDevices.Remove(networkDeviceToDelete);
  }

  async Task<NetworkDevice> IOneQueryWithRelationsRepository<NetworkDevice>.GetOneWithChildrenAsync(ISpecification<NetworkDevice> specification,
                                                                                                    CancellationToken cancellationToken)
  {
    var result = await context.Set<NetworkDevice>()
                              .AsNoTracking()
                              .ApplyIncludes(specification.IncludeChains)
                              .ApplyWhere(specification.Criterias)
                              .FirstAsync(cancellationToken);
    return result;
  }

  async Task<IEnumerable<NetworkDevice>> IManyQueryWithRelationsRepository<NetworkDevice>.GetManyWithChildrenAsync(ISpecification<NetworkDevice> specification,
                                                                                                                   CancellationToken cancellationToken)
  {
    var result = await context.Set<NetworkDevice>()
                              .AsNoTracking()
                              .ApplyIncludes(specification.IncludeChains)
                              .ApplyWhere(specification.Criterias)
                              .ApplySkip(specification.Skip)
                              .ApplyTake(specification.Take)
                              .ToListAsync(cancellationToken);
    return result;
  }

  async Task<IEnumerable<NetworkDevice>> IManyQueryRepository<NetworkDevice>.GetManyShortAsync(ISpecification<NetworkDevice> specification,
                                                                                               CancellationToken cancellationToken)
    => await manyQueryRepository.GetManyShortAsync(specification,
                                                   cancellationToken);

  async Task<bool> IExistenceQueryRepository<NetworkDevice>.AnyByQueryAsync(ISpecification<NetworkDevice> specification,
                                                                            CancellationToken cancellationToken)
    => await existenceQueryRepository.AnyByQueryAsync(specification,
                                                      cancellationToken);

  async Task<int> ICountRepository<NetworkDevice>.GetCountAsync(ISpecification<NetworkDevice> specification,
                                                                CancellationToken cancellationToken)
    => await countRepository.GetCountAsync(specification,
                                           cancellationToken);

  void IInsertRepository<NetworkDevice>.InsertOne(NetworkDevice entity)
    => insertRepository.InsertOne(entity);

  async Task<IEnumerable<NetworkDevice>> IReadRepository<NetworkDevice>.GetAllAsync(CancellationToken cancellationToken)
    => await readRepository.GetAllAsync(cancellationToken);
}
