namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class NetworkDeviceRepository(PostgreSQLDapperContext context) 
  : GenericRepository<NetworkDevice>(context), INetworkDeviceRepository
{
  void INetworkDeviceRepository.DeleteOneWithChilren(NetworkDevice networkDeviceToDelete)
  {
    _postgreSQLDapperContext.BeginTransaction();
    var connection = _postgreSQLDapperContext.CreateConnection();
    string query = string.Empty;
    // Удаление VLANs
    var vlanIds = networkDeviceToDelete.PortsOfNetworkDevice
                                       .SelectMany(x => x.VLANs)
                                       .Select(vlan => vlan.Id)
                                       .ToList();
    if (vlanIds.Count != 0)
    {
        query = $"DELETE FROM \"{GetTableName<VLAN>()}\" WHERE \"{nameof(VLAN.Id)}\" = ANY(@Ids)";
        connection.Execute(query, new { Ids = vlanIds });
        
        query = $"DELETE FROM \"{GetTableName<PortVlan>()}\" WHERE \"{nameof(PortVlan.VLANId)}\" = ANY(@Ids)";
        connection.Execute(query, new { Ids = vlanIds });
    }
    query = $"DELETE FROM \"{GetTableName<NetworkDevice>()}\" WHERE \"{nameof(NetworkDevice.Id)}\"=@Id";
    connection.Execute(query, new { Id = networkDeviceToDelete.Id });
  }

  async Task<NetworkDevice> INetworkDeviceRepository.GetOneWithChildrensByIdAsync(Guid id,
                                                                                  CancellationToken cancellationToken)
  {
    StringBuilder sb = new();
    sb.Append("SELECT ");
    sb.Append($"nd.\"{nameof(NetworkDevice.Id)}\", nd.\"{nameof(NetworkDevice.Host)}\", nd.\"{nameof(NetworkDevice.TypeOfNetworkDevice)}\", nd.\"{nameof(NetworkDevice.NetworkDeviceName)}\", nd.\"{nameof(NetworkDevice.GeneralInformation)}\", ");
    sb.Append($"p.\"{nameof(Port.Id)}\", p.\"{nameof(Port.InterfaceNumber)}\", p.\"{nameof(Port.InterfaceName)}\", p.\"{nameof(Port.InterfaceType)}\", p.\"{nameof(Port.InterfaceStatus)}\", p.\"{nameof(Port.InterfaceSpeed)}\", p.\"{nameof(Port.NetworkDeviceId)}\", p.\"{nameof(Port.ParentPortId)}\", p.\"{nameof(Port.MACAddress)}\", ");
    sb.Append($"pv.\"{nameof(PortVlan.Id)}\", pv.\"{nameof(PortVlan.PortId)}\", pv.\"{nameof(PortVlan.VLANId)}\", ");
    sb.Append($"v.\"{nameof(VLAN.Id)}\", v.\"{nameof(VLAN.VLANTag)}\", v.\"{nameof(VLAN.VLANName)}\" ");
    sb.Append($"FROM \"{GetTableName<NetworkDevice>()}\" as nd ");
    sb.Append($"LEFT JOIN \"{GetTableName<Port>()}\" AS p on p.\"{nameof(Port.NetworkDeviceId)}\" = nd.\"{nameof(NetworkDevice.Id)}\" ");
    sb.Append($"LEFT JOIN \"{GetTableName<PortVlan>()}\" AS pv on pv.\"{nameof(PortVlan.PortId)}\" = p.\"{nameof(Port.Id)}\" ");
    sb.Append($"LEFT JOIN \"{GetTableName<VLAN>()}\" AS v on v.\"{nameof(VLAN.Id)}\" = pv.\"{nameof(PortVlan.VLANId)}\" ");
    sb.Append($"WHERE nd.\"{nameof(NetworkDevice.Id)}\"=@Id");
    
    var query = sb.ToString();
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var ndDictionary = new Dictionary<Guid, NetworkDevice>();
    var pDicotionary = new Dictionary<Guid, Port>();
    var vDictionary = new Dictionary<Guid, HashSet<VLAN>>();

    await connection.QueryAsync<NetworkDevice, Port, PortVlan, VLAN, NetworkDevice>(
        query,
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
        new { Id = id },
        splitOn: "Id, Id, Id, Id");

    return ndDictionary.Values.First();
  }

  async Task<IEnumerable<NetworkDevice>> INetworkDeviceRepository.GetManyWithChildrensByVLANTagAsync(int vlanTag,
                                                                                                     CancellationToken cancellationToken)
  {
    StringBuilder sb = new();
    sb.Append("SELECT ");
    sb.Append($"nd.\"{nameof(NetworkDevice.Id)}\", nd.\"{nameof(NetworkDevice.Host)}\", nd.\"{nameof(NetworkDevice.TypeOfNetworkDevice)}\", ");
    sb.Append($"nd.\"{nameof(NetworkDevice.NetworkDeviceName)}\", nd.\"{nameof(NetworkDevice.GeneralInformation)}\", ");
    sb.Append($"p.\"{nameof(Port.Id)}\", p.\"{nameof(Port.InterfaceNumber)}\", p.\"{nameof(Port.InterfaceName)}\", p.\"{nameof(Port.InterfaceType)}\", p.\"{nameof(Port.InterfaceStatus)}\", p.\"{nameof(Port.InterfaceSpeed)}\", p.\"{nameof(Port.NetworkDeviceId)}\", p.\"{nameof(Port.ParentPortId)}\", p.\"{nameof(Port.MACAddress)}\", ");
    sb.Append($"pv.\"{nameof(PortVlan.Id)}\", pv.\"{nameof(PortVlan.PortId)}\", pv.\"{nameof(PortVlan.VLANId)}\", ");
    sb.Append($"v.\"{nameof(VLAN.Id)}\", v.\"{nameof(VLAN.VLANTag)}\", v.\"{nameof(VLAN.VLANName)}\" ");
    sb.Append($"FROM \"{GetTableName<NetworkDevice>()}\" AS nd ");
    sb.Append($"LEFT JOIN \"{GetTableName<Port>()}\" AS p ON p.\"{nameof(Port.NetworkDeviceId)}\" = nd.\"{nameof(NetworkDevice.Id)}\" ");
    sb.Append($"LEFT JOIN \"{GetTableName<PortVlan>()}\" AS pv ON pv.\"{nameof(PortVlan.PortId)}\" = p.\"{nameof(Port.Id)}\" ");
    sb.Append($"LEFT JOIN \"{GetTableName<VLAN>()}\" AS v ON v.\"{nameof(VLAN.Id)}\" = pv.\"{nameof(PortVlan.VLANId)}\" ");
    sb.Append($"WHERE v.\"{nameof(VLAN.VLANTag)}\" = @VLANTag");

    var query = sb.ToString();
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var ndDictionary = new Dictionary<Guid, NetworkDevice>();
    var pDicotionary = new Dictionary<Guid, Port>();
    var vDictionary = new Dictionary<Guid, HashSet<VLAN>>();

    await connection.QueryAsync<NetworkDevice, Port, PortVlan, VLAN, NetworkDevice>(
        query,
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
        splitOn: "Id, Id, Id, Id");

    return [.. ndDictionary.Values];
  }

  async Task<IEnumerable<NetworkDevice>> INetworkDeviceRepository.GetManyByQueryAsync(RequestParameters requestParameters,
                                                                                      CancellationToken cancellationToken)
  {
    StringBuilder sb = new();
    sb.Append("SELECT ");
    sb.Append($"nd.\"{nameof(NetworkDevice.Id)}\", nd.\"{nameof(NetworkDevice.Host)}\", nd.\"{nameof(NetworkDevice.TypeOfNetworkDevice)}\", ");
    sb.Append($"nd.\"{nameof(NetworkDevice.NetworkDeviceName)}\", nd.\"{nameof(NetworkDevice.GeneralInformation)}\" ");
    sb.Append($"FROM \"{GetTableName<NetworkDevice>()}\" AS nd ");

    var baseQuery = sb.ToString();
    var queryBuilder = new SqlQueryBuilder(requestParameters,
                                           "nd",
                                           typeof(NetworkDevice));
    var (finalQuery, parameters) = queryBuilder.BuildBaseQuery(baseQuery);
    
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    return await connection.QueryAsync<NetworkDevice>(finalQuery, parameters);
  }
}
