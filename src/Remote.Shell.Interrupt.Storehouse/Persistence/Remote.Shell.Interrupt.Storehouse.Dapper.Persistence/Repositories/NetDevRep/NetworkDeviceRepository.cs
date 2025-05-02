namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.NetDevRep;

internal class NetworkDeviceRepository(PostgreSQLDapperContext context,
                                       IAppLogger<NetworkDeviceRepository> logger,
                                       IManyQueryRepository<NetworkDevice> manyQueryRepository,
                                       IExistenceQueryRepository<NetworkDevice> existenceQueryRepository,
                                       ICountRepository<NetworkDevice> countRepository,
                                       IInsertRepository<NetworkDevice> insertRepository,
                                       IReadRepository<NetworkDevice> readRepository) 
  : INetworkDeviceRepository
{
  void INetworkDeviceRepository.DeleteOneWithChilren(NetworkDevice networkDeviceToDelete)
  {
    context.BeginTransaction();
    var connection = context.CreateConnection();
    string query = string.Empty;
    // Удаление VLANs
    var vlanIds = networkDeviceToDelete.PortsOfNetworkDevice
                                       .SelectMany(x => x.VLANs)
                                       .Select(vlan => vlan.Id)
                                       .ToList();
    if (vlanIds.Count != 0)
    {
        query = $"DELETE FROM \"{GetTableName.Handle<VLAN>()}\" WHERE \"{nameof(VLAN.Id)}\" = ANY(@Ids)";
        connection.Execute(query, new { Ids = vlanIds });
        
        query = $"DELETE FROM \"{GetTableName.Handle<PortVlan>()}\" WHERE \"{nameof(PortVlan.VLANId)}\" = ANY(@Ids)";
        connection.Execute(query, new { Ids = vlanIds });
    }
    query = $"DELETE FROM \"{GetTableName.Handle<NetworkDevice>()}\" WHERE \"{nameof(NetworkDevice.Id)}\"=@Id";
    
    logger.LogInformation(query);
    
    connection.Execute(query, new { Id = networkDeviceToDelete.Id });
  }

  async Task<NetworkDevice> IOneQueryWithRelationsRepository<NetworkDevice>.GetOneWithChildrenAsync(ISpecification<NetworkDevice> specification,
                                                                                                    CancellationToken cancellationToken)
  {
    var queryBuilder = new SqlQueryBuilder<NetworkDevice>(specification);

    var sql = queryBuilder.Build();

    logger.LogInformation(sql);
    
    var connection = await context.CreateConnectionAsync(cancellationToken);
    
    var ndDictionary = new Dictionary<Guid, NetworkDevice>();
    var pDicotionary = new Dictionary<Guid, Port>();
    var vDictionary = new Dictionary<Guid, HashSet<VLAN>>();

    await connection.QueryAsync<NetworkDevice, Port, PortVlan, VLAN, NetworkDevice>(
        sql,
        (nd, p, pv, v) =>
        {
          if (!ndDictionary.TryGetValue(nd.Id, out var networkDeviceEntry))
          {
            networkDeviceEntry = nd;
            ndDictionary.Add(networkDeviceEntry.Id, networkDeviceEntry);
          }

          if (!pDicotionary.TryGetValue(p.Id, out var portEntry))
          {
            portEntry = p;
            networkDeviceEntry.PortsOfNetworkDevice.Add(p);
            pDicotionary.Add(portEntry.Id, portEntry);
          }

          // Добавление VLAN в PortsOfNetworkDevice
          if (v is not null && !portEntry.VLANs.Any(x => x.Id == v.Id))
            portEntry.VLANs.Add(v);

          return networkDeviceEntry;
        },
        splitOn: $"{nameof(NetworkDevice.Id)},{nameof(Port.Id)},{nameof(PortVlan.Id)},{nameof(VLAN.Id)}");

    return ndDictionary.Values.First();
  }

  async Task<IEnumerable<NetworkDevice>> IManyQueryWithRelationsRepository<NetworkDevice>.GetManyWithChildrenAsync(ISpecification<NetworkDevice> specification,
                                                                                                                   CancellationToken cancellationToken)
  {
    var queryBuilder = new SqlQueryBuilder<NetworkDevice>(specification);

    var sql = queryBuilder.Build();

    logger.LogInformation(sql);
    
    var connection = await context.CreateConnectionAsync(cancellationToken);

    var ndDictionary = new Dictionary<Guid, NetworkDevice>();
    var pDicotionary = new Dictionary<Guid, Port>();
    var vDictionary = new Dictionary<Guid, HashSet<VLAN>>();

    await connection.QueryAsync<NetworkDevice, Port, PortVlan, VLAN, NetworkDevice>(
        sql,
        (nd, p, pv, v) =>
        {
          if (!ndDictionary.TryGetValue(nd.Id, out var networkDeviceEntry))
          {
            networkDeviceEntry = nd;
            ndDictionary.Add(networkDeviceEntry.Id, networkDeviceEntry);
          }

          if (!pDicotionary.TryGetValue(p.Id, out var portEntry))
          {
            portEntry = p;
            networkDeviceEntry.PortsOfNetworkDevice.Add(p);
            pDicotionary.Add(portEntry.Id, portEntry);
          }

          // Добавление VLAN в PortsOfNetworkDevice
          if (v is not null && !portEntry.VLANs.Any(x => x.Id == v.Id))
            portEntry.VLANs.Add(v);

          return networkDeviceEntry;
        },
        splitOn: $"{nameof(NetworkDevice.Id)},{nameof(Port.Id)},{nameof(PortVlan.Id)},{nameof(VLAN.Id)}");

    return ndDictionary.Values;
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
