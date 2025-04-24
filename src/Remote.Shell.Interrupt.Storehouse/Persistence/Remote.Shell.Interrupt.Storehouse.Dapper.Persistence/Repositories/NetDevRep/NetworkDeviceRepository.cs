namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.NetDevRep;

internal class NetworkDeviceRepository(PostgreSQLDapperContext context,
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
    connection.Execute(query, new { Id = networkDeviceToDelete.Id });
  }

  async Task<NetworkDevice> IOneQueryWithRelationsRepository<NetworkDevice>.GetOneWithChildrensAsync(RequestParameters requestParameters,
                                                                                                     CancellationToken cancellationToken)
  {
    StringBuilder sb = new();
    sb.Append("SELECT ");
    sb.Append($"nd.\"{nameof(NetworkDevice.Id)}\", nd.\"{nameof(NetworkDevice.Host)}\", nd.\"{nameof(NetworkDevice.TypeOfNetworkDevice)}\", ");
    sb.Append($"nd.\"{nameof(NetworkDevice.NetworkDeviceName)}\", nd.\"{nameof(NetworkDevice.GeneralInformation)}\", ");
    sb.Append($"p.\"{nameof(Port.Id)}\", p.\"{nameof(Port.InterfaceNumber)}\", p.\"{nameof(Port.InterfaceName)}\", ");
    sb.Append($"p.\"{nameof(Port.InterfaceType)}\", p.\"{nameof(Port.InterfaceStatus)}\", p.\"{nameof(Port.InterfaceSpeed)}\", ");
    sb.Append($"p.\"{nameof(Port.NetworkDeviceId)}\", p.\"{nameof(Port.ParentPortId)}\", p.\"{nameof(Port.MACAddress)}\", ");
    sb.Append($"pv.\"{nameof(PortVlan.Id)}\", pv.\"{nameof(PortVlan.PortId)}\", pv.\"{nameof(PortVlan.VLANId)}\", ");
    sb.Append($"v.\"{nameof(VLAN.Id)}\", v.\"{nameof(VLAN.VLANTag)}\", v.\"{nameof(VLAN.VLANName)}\" ");
    sb.Append($"FROM \"{GetTableName.Handle<NetworkDevice>()}\" as nd ");
    sb.Append($"LEFT JOIN \"{GetTableName.Handle<Port>()}\" AS p on p.\"{nameof(Port.NetworkDeviceId)}\" = nd.\"{nameof(NetworkDevice.Id)}\" ");
    sb.Append($"LEFT JOIN \"{GetTableName.Handle<PortVlan>()}\" AS pv on pv.\"{nameof(PortVlan.PortId)}\" = p.\"{nameof(Port.Id)}\" ");
    sb.Append($"LEFT JOIN \"{GetTableName.Handle<VLAN>()}\" AS v on v.\"{nameof(VLAN.Id)}\" = pv.\"{nameof(PortVlan.VLANId)}\" ");
    sb.Append($"WHERE nd.\"{nameof(NetworkDevice.Id)}\"=@Id");
    
    var baseQuery = sb.ToString();
    var connection = await context.CreateConnectionAsync(cancellationToken);
    var ndDictionary = new Dictionary<Guid, NetworkDevice>();
    var pDicotionary = new Dictionary<Guid, Port>();
    var vDictionary = new Dictionary<Guid, HashSet<VLAN>>();

    var queryBuilder = new SqlQueryBuilder(requestParameters,
                                           "nd",
                                           typeof(NetworkDevice));

    var (finalQuery, parameters) = queryBuilder.BuildBaseQuery(baseQuery, true);

    await connection.QueryAsync<NetworkDevice, Port, PortVlan, VLAN, NetworkDevice>(
        finalQuery,
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
        parameters,
        splitOn: $"{nameof(NetworkDevice.Id)}, {nameof(Port.Id)}, {nameof(PortVlan.Id)}, {nameof(VLAN.Id)}");

    return ndDictionary.Values.First();
  }

  async Task<IEnumerable<NetworkDevice>> INetworkDeviceRepository.GetManyWithChildrenByVlanTagAsync(int vlanTag,
                                                                                                    CancellationToken cancellationToken)
  {
    StringBuilder sb = new();
    sb.Append("SELECT ");
    sb.Append($"nd.\"{nameof(NetworkDevice.Id)}\", nd.\"{nameof(NetworkDevice.Host)}\", nd.\"{nameof(NetworkDevice.TypeOfNetworkDevice)}\", ");
    sb.Append($"nd.\"{nameof(NetworkDevice.NetworkDeviceName)}\", nd.\"{nameof(NetworkDevice.GeneralInformation)}\", ");
    sb.Append($"p.\"{nameof(Port.Id)}\", p.\"{nameof(Port.InterfaceNumber)}\", p.\"{nameof(Port.InterfaceName)}\", ");
    sb.Append($"p.\"{nameof(Port.InterfaceType)}\", p.\"{nameof(Port.InterfaceStatus)}\", p.\"{nameof(Port.InterfaceSpeed)}\", ");
    sb.Append($"p.\"{nameof(Port.NetworkDeviceId)}\", p.\"{nameof(Port.ParentPortId)}\", p.\"{nameof(Port.MACAddress)}\", ");
    sb.Append($"pv.\"{nameof(PortVlan.Id)}\", pv.\"{nameof(PortVlan.PortId)}\", pv.\"{nameof(PortVlan.VLANId)}\", ");
    sb.Append($"v.\"{nameof(VLAN.Id)}\", v.\"{nameof(VLAN.VLANTag)}\", v.\"{nameof(VLAN.VLANName)}\" ");
    sb.Append($"FROM \"{GetTableName.Handle<NetworkDevice>()}\" AS nd ");
    sb.Append($"LEFT JOIN \"{GetTableName.Handle<Port>()}\" AS p ON p.\"{nameof(Port.NetworkDeviceId)}\" = nd.\"{nameof(NetworkDevice.Id)}\" ");
    sb.Append($"LEFT JOIN \"{GetTableName.Handle<PortVlan>()}\" AS pv ON pv.\"{nameof(PortVlan.PortId)}\" = p.\"{nameof(Port.Id)}\" ");
    sb.Append($"LEFT JOIN \"{GetTableName.Handle<VLAN>()}\" AS v ON v.\"{nameof(VLAN.Id)}\" = pv.\"{nameof(PortVlan.VLANId)}\" ");
    sb.Append($"WHERE v.\"{nameof(VLAN.VLANTag)}\" = @VLANTag");

    var baseQuery = sb.ToString();
    var connection = await context.CreateConnectionAsync(cancellationToken);
    var ndDictionary = new Dictionary<Guid, NetworkDevice>();
    var pDicotionary = new Dictionary<Guid, Port>();
    var vDictionary = new Dictionary<Guid, HashSet<VLAN>>();

    await connection.QueryAsync<NetworkDevice, Port, PortVlan, VLAN, NetworkDevice>(
        baseQuery,
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
        new { VLANTag = vlanTag },
        splitOn: $"{nameof(NetworkDevice.Id)}, {nameof(Port.Id)}, {nameof(PortVlan.Id)}, {nameof(VLAN.Id)}");

    return ndDictionary.Values;
  }

  async Task<IEnumerable<NetworkDevice>> IManyQueryRepository<NetworkDevice>.GetManyShortAsync(RequestParameters requestParameters,
                                                                                               CancellationToken cancellationToken,
                                                                                               bool skipFiltering)
    => await manyQueryRepository.GetManyShortAsync(requestParameters,
                                                   cancellationToken,
                                                   skipFiltering);

  async Task<bool> IExistenceQueryRepository<NetworkDevice>.AnyByQueryAsync(RequestParameters requestParameters,
                                                                            CancellationToken cancellationToken)
    => await existenceQueryRepository.AnyByQueryAsync(requestParameters,
                                                      cancellationToken);

  async Task<int> ICountRepository<NetworkDevice>.GetCountAsync(RequestParameters requestParameters,
                                                                CancellationToken cancellationToken)
    => await countRepository.GetCountAsync(requestParameters,
                                           cancellationToken);

  void IInsertRepository<NetworkDevice>.InsertOne(NetworkDevice entity)
    => insertRepository.InsertOne(entity);

  async Task<IEnumerable<NetworkDevice>> IReadRepository<NetworkDevice>.GetAllAsync(CancellationToken cancellationToken)
    => await readRepository.GetAllAsync(cancellationToken);

  public Task<IEnumerable<NetworkDevice>> GetManyWithChildrenAsync(RequestParameters requestParameters, CancellationToken cancellationToken)
  {
    throw new NotImplementedException("Oops! Crutch №1 =)");
  }
}
