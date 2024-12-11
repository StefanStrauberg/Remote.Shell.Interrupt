namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class NetworkDeviceRepository(PostgreSQLDapperContext context) : GenericRepository<NetworkDevice>(context), INetworkDeviceRepository
{
  void INetworkDeviceRepository.DeleteOneWithChilren(NetworkDevice networkDeviceToDelete)
  {
    _postgreSQLDapperContext.BeginTransaction();
    var connection = _postgreSQLDapperContext.CreateConnection();
    var vlans = networkDeviceToDelete.PortsOfNetworkDevice.SelectMany(x => x.VLANs);
    var query = $"DELETE FROM \"VLANs\" WHERE \"Id\"=@Id";
    foreach (var vlan in vlans)
    {
      connection.Execute(query, new { Id = vlan.Id });
    }
    query = $"DELETE FROM \"PortVlans\" WHERE \"VLANId\"=@Id";
    foreach (var vlan in vlans)
    {
      connection.Execute(query, new { Id = vlan.Id });
    }
    query = $"DELETE FROM \"NetworkDevices\" WHERE \"Id\"=@Id";
    connection.Execute(query, new { Id = networkDeviceToDelete.Id });
  }

  async Task<NetworkDevice> INetworkDeviceRepository.GetFirstWithChildrensByIdAsync(Guid id,
                                                                                    CancellationToken cancellationToken)
  {
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);

    var query = $"SELECT " +
                 "nd.\"Id\", nd.\"Host\", nd.\"TypeOfNetworkDevice\", nd.\"NetworkDeviceName\", nd.\"GeneralInformation\", " +
                 "p.\"Id\", p.\"InterfaceNumber\", p.\"InterfaceName\", p.\"InterfaceType\", p.\"InterfaceStatus\", p.\"InterfaceSpeed\", p.\"NetworkDeviceId\", p.\"ParentPortId\", p.\"MACAddress\", " +
                 "pv.\"Id\", pv.\"PortId\", pv.\"VLANId\", " +
                 "v.\"Id\", v.\"VLANTag\", v.\"VLANName\" " +
                 "FROM \"NetworkDevices\" as nd " +
                 "LEFT JOIN \"Ports\" AS p on p.\"NetworkDeviceId\" = nd.\"Id\" " +
                 "LEFT JOIN \"PortVlans\" AS pv on pv.\"PortId\" = p.\"Id\" " +
                 "LEFT JOIN \"VLANs\" AS v on v.\"Id\" = pv.\"VLANId\" " +
                 "WHERE nd.\"Id\"=@Id";

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

  async Task<IEnumerable<NetworkDevice>> INetworkDeviceRepository.GetAllWithChildrensByVLANTagAsync(int vlanTag,
                                                                                                    CancellationToken cancellationToken)
  {
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);

    var query = $"SELECT " +
                 "nd.\"Id\", nd.\"Host\", nd.\"TypeOfNetworkDevice\", nd.\"NetworkDeviceName\", nd.\"GeneralInformation\", " +
                 "p.\"Id\", p.\"InterfaceNumber\", p.\"InterfaceName\", p.\"InterfaceType\", p.\"InterfaceStatus\", p.\"InterfaceSpeed\", p.\"NetworkDeviceId\", p.\"ParentPortId\", p.\"MACAddress\", " +
                 "pv.\"Id\", pv.\"PortId\", pv.\"VLANId\", " +
                 "v.\"Id\", v.\"VLANTag\", v.\"VLANName\" " +
                 "FROM \"NetworkDevices\" AS nd " +
                 "LEFT JOIN \"Ports\" AS p ON p.\"NetworkDeviceId\" = nd.\"Id\" " +
                 "LEFT JOIN \"PortVlans\" AS pv ON pv.\"PortId\" = p.\"Id\" " +
                 "LEFT JOIN \"VLANs\" AS v ON v.\"Id\" = pv.\"VLANId\" " +
                 "WHERE v.\"VLANTag\" = @VLANTag";

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
}
