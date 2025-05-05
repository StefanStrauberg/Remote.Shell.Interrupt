namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.NetDevRep;

internal class PortRepository(ApplicationDbContext context,
                              IExistenceQueryRepository<Port> existenceQueryRepository,
                              IOneQueryRepository<Port> oneQueryRepository,
                              IBulkInsertRepository<Port> bulkInsertRepository,
                              IBulkDeleteRepository<Port> bulkDeleteRepository, 
                              IBulkReplaceRepository<Port> bulkReplaceRepository)
  : IPortRepository
{
  async Task<IEnumerable<Port>> IPortRepository.GetAllAggregatedPortsByListAsync(IEnumerable<Guid> Ids,
                                                                                 CancellationToken cancellationToken)
  {
    var result = await context.Set<Port>()
                              .Where(p => Ids.ToList().Contains(p.ParentPortId!.Value))
                              .ToListAsync();
    return result;
  }

  async Task<string> IPortRepository.LookingForInterfaceNameByIPAsync(string ipAddress,
                                                                      CancellationToken cancellationToken)
  {   
    StringBuilder sb = new();
    sb.Append($"SELECT p.\"{nameof(Port.InterfaceName)}\" ");
    sb.Append($"FROM \"{GetTableName.Handle<Port>()}\" AS p ");
    sb.Append($"LEFT JOIN \"{GetTableName.Handle<TerminatedNetworkEntity>()}\" AS tn ON tn.\"{nameof(TerminatedNetworkEntity.PortId)}\" = p.\"{nameof(Port.Id)}\" ");
    sb.Append($"WHERE tn.\"{nameof(TerminatedNetworkEntity.NetworkAddress)}\" IS NOT NULL ");
    sb.Append($"AND tn.\"{nameof(TerminatedNetworkEntity.Netmask)}\" IS NOT NULL ");
    sb.Append($"AND tn.\"{nameof(TerminatedNetworkEntity.NetworkAddress)}\" <> 0 ");
    sb.Append($"AND tn.\"{nameof(TerminatedNetworkEntity.Netmask)}\" <> 0 ");
    sb.Append($"AND ((tn.\"{nameof(TerminatedNetworkEntity.NetworkAddress)}\" & tn.\"{nameof(TerminatedNetworkEntity.Netmask)}\") = ((@IP::inet - '0.0.0.0'::inet) & tn.\"{nameof(TerminatedNetworkEntity.Netmask)}\"))");
    
    var result = await context.Set<Port>()
                              .FromSqlRaw(sb.ToString())
                              .Select(x => x.InterfaceName)
                              .ExecuteRawQueryAsync();

    return result.Select(x => x.InterfaceName)
                 .First();
  }

  async Task<IEnumerable<Port>> IPortRepository.GetPortsWithMacAddressesAndSpecificHostsAsync(string MACAddress,
                                                                                              List<string> hosts,
                                                                                              CancellationToken cancellationToken)
  {
    var hostsToDelete = GetStringHosts.Handle(hosts);
    StringBuilder sb = new();
    sb.Append($"SELECT \"{nameof(Port.Id)}\", \"{nameof(Port.InterfaceNumber)}\", \"{nameof(Port.InterfaceName)}\", ");
    sb.Append($"\"{nameof(Port.InterfaceType)}\", \"{nameof(Port.InterfaceStatus)}\", \"{nameof(Port.InterfaceSpeed)}\", ");
    sb.Append($"\"{nameof(Port.NetworkDeviceId)}\", \"{nameof(Port.ParentPortId)}\",  \"{nameof(Port.MACAddress)}\", ");
    sb.Append($"\"{nameof(Port.Description)}\" ");
    sb.Append($"FROM \"{GetTableName.Handle<NetworkDevice>()}\" AS nd ");
    sb.Append($"LEFT JOIN \"{GetTableName.Handle<Port>()}\" AS p ON p.\"{nameof(Port.NetworkDeviceId)}\" = nd.\"{nameof(NetworkDevice.Id)}\" ");
    sb.Append($"LEFT JOIN \"{GetTableName.Handle<MACEntity>()}\" AS mac ON mac.\"{nameof(MACEntity.PortId)}\" = p.\"{nameof(Port.Id)}\" ");
    sb.Append($"WHERE mac.\"{nameof(MACEntity.MACAddress)}\" = @MAC ");
    sb.Append($"AND nd.\"{nameof(NetworkDevice.Host)}\" in ({hostsToDelete})");

    var result = await context.Set<Port>()
                              .FromSqlRaw(sb.ToString())
                              .ExecuteRawQueryAsync();

    return result;
  }

  async Task<bool> IExistenceQueryRepository<Port>.AnyByQueryAsync(ISpecification<Port> specification,
                                                                   CancellationToken cancellationToken)
    => await existenceQueryRepository.AnyByQueryAsync(specification,
                                                      cancellationToken);

  async Task<Port> IOneQueryRepository<Port>.GetOneShortAsync(ISpecification<Port> specification,
                                                              CancellationToken cancellationToken)
    => await oneQueryRepository.GetOneShortAsync(specification,
                                                 cancellationToken);

  void IBulkInsertRepository<Port>.InsertMany(IEnumerable<Port> entities)
    => bulkInsertRepository.InsertMany(entities);

  void IBulkDeleteRepository<Port>.DeleteMany(IEnumerable<Port> entities)
    => bulkDeleteRepository.DeleteMany(entities);

  void IBulkReplaceRepository<Port>.ReplaceMany(IEnumerable<Port> entities)
    => bulkReplaceRepository.ReplaceMany(entities);
}