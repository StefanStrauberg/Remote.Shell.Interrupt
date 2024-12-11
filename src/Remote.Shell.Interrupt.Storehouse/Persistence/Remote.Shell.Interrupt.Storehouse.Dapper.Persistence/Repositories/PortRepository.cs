

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class PortRepository(PostgreSQLDapperContext context) : GenericRepository<Port>(context), IPortRepository
{
  async Task<IEnumerable<Port>> IPortRepository.GetAllAggregatedPortsByListAsync(List<Guid> Ids,
                                                                                 CancellationToken cancellationToken)
  {
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var ids = GetStringIds(Ids);
    var query = "SELECT \"Id\", \"InterfaceNumber\", \"InterfaceName\", \"InterfaceType\", \"InterfaceStatus\", \"InterfaceSpeed\", \"NetworkDeviceId\", \"ParentPortId\",  \"MACAddress\", \"Description\" " +
                "FROM \"Ports\" " +
                $"WHERE \"ParentPortId\" IN ({ids})";
    return await connection.QueryAsync<Port>(query);
  }

  async Task<IEnumerable<Port>> IPortRepository.GetAllAggregatedPortsByIdAsync(Guid id,
                                                                             CancellationToken cancellationToken)
  {
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var query = "SELECT \"Id\", \"InterfaceNumber\", \"InterfaceName\", \"InterfaceType\", \"InterfaceStatus\", \"InterfaceSpeed\", \"NetworkDeviceId\", \"ParentPortId\",  \"MACAddress\", \"Description\" " +
                "FROM \"Ports\" " +
                "WHERE \"ParentPortId\" = @Id";
    return await connection.QueryAsync<Port>(query, new { Id = id });
  }

  async Task<string> IPortRepository.LookingForInterfaceNameByIPAsync(string ipAddress,
                                                                      CancellationToken cancellationToken)
  {
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var query = "SELECT p.\"InterfaceName\" FROM \"Ports\" AS p " +
                "LEFT JOIN \"TerminatedNetworkEntities\" AS tn ON tn.\"PortId\" = p.\"Id\" " +
                "WHERE tn.\"NetworkAddress\" IS NOT NULL " +
                "AND tn.\"Netmask\" IS NOT NULL " +
                "AND tn.\"NetworkAddress\" <> 0 " +
                "AND tn.\"Netmask\" <> 0 " +
                "AND ((tn.\"NetworkAddress\" & tn.\"Netmask\") = ((@IP::inet - '0.0.0.0'::inet) & tn.\"Netmask\"))";
    var reuslt = await connection.ExecuteScalarAsync<string>(query, new { IP = ipAddress });
    return reuslt ?? string.Empty;
  }

  static string GetStringIds(List<Guid> ids)
  {
    var sb = new StringBuilder();
    for (int i = 0; i < ids.Count; i++)
    {
      sb.Append('\'');
      sb.Append(ids[i].ToString());
      sb.Append('\'');
      if (i != ids.Count - 1)
        sb.Append(',');
    }
    return sb.ToString();
  }

  static string GetStringHosts(List<string> hosts)
  {
    var sb = new StringBuilder();
    for (int i = 0; i < hosts.Count; i++)
    {
      sb.Append('\'');
      sb.Append(hosts[i].ToString());
      sb.Append('\'');
      if (i != hosts.Count - 1)
        sb.Append(',');
    }
    return sb.ToString();
  }

  async Task<Port> IPortRepository.GetPortWithNameAsync(string name,
                                                        CancellationToken cancellationToken)
  {
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var query = "SELECT \"Id\", \"InterfaceNumber\", \"InterfaceName\", \"InterfaceType\", \"InterfaceStatus\", \"InterfaceSpeed\", \"NetworkDeviceId\", \"ParentPortId\",  \"MACAddress\", \"Description\" " +
                "FROM \"Ports\" " +
                $"WHERE \"InterfaceName\" LIKE '%{name}%'";
    return await connection.QueryFirstAsync<Port>(query);
  }

  async Task<IEnumerable<Port>> IPortRepository.GetPortsWithWithMacAddressesAndSpecificHostsAsync(string MACAddress,
                                                                                                  List<string> hosts,
                                                                                                  CancellationToken cancellationToken)
  {
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var hostsToDelete = GetStringHosts(hosts);
    var query = "SELECT p.\"Id\", p.\"InterfaceNumber\", p.\"InterfaceName\", p.\"InterfaceType\", p.\"InterfaceStatus\", p.\"InterfaceSpeed\", p.\"NetworkDeviceId\", p.\"ParentPortId\",  p.\"MACAddress\", p.\"Description\" " +
                "FROM \"NetworkDevices\" AS nd " +
                "LEFT JOIN \"Ports\" AS p ON p.\"NetworkDeviceId\" = nd.\"Id\" " +
                "LEFT JOIN \"MACEntities\" AS mac ON mac.\"PortId\" = p.\"Id\" " +
                "WHERE mac.\"MACAddress\" = @MAC " +
                $"AND nd.\"Host\" in ({hostsToDelete})";
    return await connection.QueryAsync<Port>(query, new { MAC = MACAddress });
  }
}