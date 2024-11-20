
using System.Collections;

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class NetworkDeviceRepository(DapperContext context) : GenericRepository<NetworkDevice>(context), INetworkDeviceRepository
{
  public Task<IEnumerable<NetworkDevice>> FindManyWithChildrenAsync(Expression<Func<NetworkDevice, bool>> filterExpression, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task<IEnumerable<NetworkDevice>> GetAllWithChildrenAsync(CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public async Task<NetworkDevice> GetFirstWithChildrensByIdAsync(Guid id,
                                                                  CancellationToken cancellationToken)
  {
    string tableName = GetTableName();
    string columns = GetColumnsAsProperties();
    var connection = await _context.CreateConnectionAsync(cancellationToken);

    var query = $"SELECT " +
                 "nd.\"Id\", nd.\"Host\", nd.\"TypeOfNetworkDevice\", nd.\"NetworkDeviceName\", nd.\"GeneralInformation\", " +
                 "p.\"Id\", p.\"InterfaceNumber\", p.\"InterfaceName\", p.\"InterfaceType\", p.\"InterfaceStatus\", p.\"InterfaceSpeed\", p.\"NetworkDeviceId\", p.\"ParentPortId\", p.\"MACAddress\", " +
                 "arp.\"Id\", arp.\"MAC\", arp.\"IPAddress\", arp.\"PortId\", " +
                 "mac.\"Id\", mac.\"MACAddress\", mac.\"PortId\", " +
                 "tn.\"Id\", tn.\"NetworkAddress\", tn.\"Netmask\", tn.\"PortId\", " +
                 "pv.\"Id\", pv.\"PortId\", pv.\"VLANId\", " +
                 "v.\"Id\", v.\"VLANTag\", v.\"VLANName\" " +
                 "FROM \"NetworkDevices\" as nd " +
                 "LEFT JOIN \"Ports\" AS p on p.\"NetworkDeviceId\" = nd.\"Id\" " +
                 "LEFT JOIN \"ARPEntities\" AS arp on arp.\"PortId\" = p.\"Id\" " +
                 "LEFT JOIN \"MACEntities\" AS mac on mac.\"PortId\" = p.\"Id\" " +
                 "LEFT JOIN \"TerminatedNetworkEntities\" AS tn on tn.\"PortId\" = p.\"Id\" " +
                 "LEFT JOIN \"PortVlans\" AS pv on pv.\"PortId\" = p.\"Id\" " +
                 "LEFT JOIN \"VLANs\" AS v on v.\"Id\" = pv.\"VLANId\" " +
                 "WHERE nd.\"Id\"=@Id";

    var ndDictionary = new Dictionary<Guid, NetworkDevice>();
    var pDicotionary = new Dictionary<Guid, Port>();
    var arpDictionary = new Dictionary<Guid, ARPEntity>();
    var macDictionary = new Dictionary<Guid, MACEntity>();
    var tnDictionary = new Dictionary<Guid, TerminatedNetworkEntity>();
    var vDictionary = new Dictionary<Guid, HashSet<VLAN>>();

    await connection.QueryAsync<NetworkDevice, Port, ARPEntity, MACEntity, TerminatedNetworkEntity, PortVlan, VLAN, NetworkDevice>(
        query,
        (nd, p, arp, mac, tn, pv, v) =>
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

          if (arp is not null && !arpDictionary.TryGetValue(arp.Id, out var arpEntry))
          {
            arpEntry = arp;
            portEntry.ARPTableOfInterface.Add(arp);
            arpDictionary.Add(arpEntry.Id, arpEntry);
          }

          if (mac is not null && !macDictionary.TryGetValue(mac.Id, out var macEntry))
          {
            macEntry = mac;
            portEntry.MACTable.Add(mac);
            macDictionary.Add(macEntry.Id, macEntry);
          }

          if (tn is not null && !tnDictionary.TryGetValue(tn.Id, out var terminatedNetworkEntry))
          {
            terminatedNetworkEntry = tn;
            portEntry.NetworkTableOfInterface.Add(tn);
            tnDictionary.Add(terminatedNetworkEntry.Id, terminatedNetworkEntry);
          }

          // Добавление VLAN в PortsOfNetworkDevice
          if (v is not null && !portEntry.VLANs.Any(x => x.Id == v.Id))
            portEntry.VLANs.Add(v);

          return networkDeviceEntry;
        },
        new { Id = id },
        splitOn: "Id, Id, Id, Id, Id, Id, Id");

    return ndDictionary.Values.First();
  }
}
