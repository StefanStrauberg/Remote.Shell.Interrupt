namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class PortRepository(PostgreSQLDapperContext context) 
  : GenericRepository<Port>(context), IPortRepository
{
  async Task<IEnumerable<Port>> IPortRepository.GetAllAggregatedPortsByListAsync(List<Guid> Ids,
                                                                                 CancellationToken cancellationToken)
  {
    var ids = GetStringIds(Ids);
    StringBuilder sb = new();
    sb.Append($"SELECT \"{nameof(Port.Id)}\", \"{nameof(Port.InterfaceNumber)}\", \"{nameof(Port.InterfaceName)}\", \"{nameof(Port.InterfaceType)}\", \"{nameof(Port.InterfaceStatus)}\", \"{nameof(Port.InterfaceSpeed)}\", \"{nameof(Port.NetworkDeviceId)}\", \"{nameof(Port.ParentPortId)}\",  \"{nameof(Port.MACAddress)}\", \"{nameof(Port.Description)}\" ");
    sb.Append($"FROM \"{GetTableName<Port>()}\" ");
    sb.Append($"WHERE \"{nameof(Port.ParentPortId)}\" IN ({ids})");

    var query = sb.ToString();
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);

    return await connection.QueryAsync<Port>(query);
  }

  async Task<IEnumerable<Port>> IPortRepository.GetAllAggregatedPortsByIdAsync(Guid id,
                                                                               CancellationToken cancellationToken)
  {
    StringBuilder sb = new();
    sb.Append($"SELECT \"{nameof(Port.Id)}\", \"{nameof(Port.InterfaceNumber)}\", \"{nameof(Port.InterfaceName)}\", \"{nameof(Port.InterfaceType)}\", \"{nameof(Port.InterfaceStatus)}\", \"{nameof(Port.InterfaceSpeed)}\", \"{nameof(Port.NetworkDeviceId)}\", \"{nameof(Port.ParentPortId)}\",  \"{nameof(Port.MACAddress)}\", \"{nameof(Port.Description)}\" ");
    sb.Append($"FROM \"{GetTableName<Port>()}\" ");
    sb.Append($"WHERE \"{nameof(Port.ParentPortId)}\" = @Id");

    var query = sb.ToString();
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);

    return await connection.QueryAsync<Port>(query, new { Id = id });
  }

  async Task<string> IPortRepository.LookingForInterfaceNameByIPAsync(string ipAddress,
                                                                      CancellationToken cancellationToken)
  {
    StringBuilder sb = new();
    sb.Append($"SELECT p.\"{nameof(Port.InterfaceName)}\" ");
    sb.Append($"FROM \"{GetTableName<Port>()}\" AS p ");
    sb.Append($"LEFT JOIN \"{GetTableName<TerminatedNetworkEntity>()}\" AS tn ON tn.\"{nameof(TerminatedNetworkEntity.PortId)}\" = p.\"{nameof(Port.Id)}\" ");
    sb.Append($"WHERE tn.\"{nameof(TerminatedNetworkEntity.NetworkAddress)}\" IS NOT NULL ");
    sb.Append($"AND tn.\"{nameof(TerminatedNetworkEntity.Netmask)}\" IS NOT NULL ");
    sb.Append($"AND tn.\"{nameof(TerminatedNetworkEntity.NetworkAddress)}\" <> 0 ");
    sb.Append($"AND tn.\"{nameof(TerminatedNetworkEntity.Netmask)}\" <> 0 ");
    sb.Append($"AND ((tn.\"{nameof(TerminatedNetworkEntity.NetworkAddress)}\" & tn.\"{nameof(TerminatedNetworkEntity.Netmask)}\") = ((@IP::inet - '0.0.0.0'::inet) & tn.\"{nameof(TerminatedNetworkEntity.Netmask)}\"))");

    var query = sb.ToString();
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    return await connection.ExecuteScalarAsync<string>(query, new { IP = ipAddress }) ?? string.Empty;
  }

  static string GetStringIds(List<Guid> ids)
  {
    var sb = new StringBuilder();

    for (int i = 0; i < ids.Count; i++)
    {
      sb.Append('\'');
      sb.Append(ids[i]);
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
      sb.Append(hosts[i]);
      sb.Append('\'');

      if (i != hosts.Count - 1)
        sb.Append(',');
    }

    return sb.ToString();
  }

  async Task<Port> IPortRepository.GetPortWithNameAsync(string name,
                                                        CancellationToken cancellationToken)
  {
    StringBuilder sb = new();
    sb.Append($"SELECT \"{nameof(Port.Id)}\", \"{nameof(Port.InterfaceNumber)}\", \"{nameof(Port.InterfaceName)}\", \"{nameof(Port.InterfaceType)}\", \"{nameof(Port.InterfaceStatus)}\", \"{nameof(Port.InterfaceSpeed)}\", \"{nameof(Port.NetworkDeviceId)}\", \"{nameof(Port.ParentPortId)}\",  \"{nameof(Port.MACAddress)}\", \"{nameof(Port.Description)}\" ");
    sb.Append($"FROM \"{GetTableName<Port>()}\" ");
    sb.Append($"WHERE \"{nameof(Port.InterfaceName)}\" LIKE '%{name}%'");

    var query = sb.ToString();
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    
    return await connection.QueryFirstAsync<Port>(query);
  }

  async Task<IEnumerable<Port>> IPortRepository.GetPortsWithWithMacAddressesAndSpecificHostsAsync(string MACAddress,
                                                                                                  List<string> hosts,
                                                                                                  CancellationToken cancellationToken)
  {
    var hostsToDelete = GetStringHosts(hosts);
    StringBuilder sb = new();
    sb.Append($"SELECT p.\"{nameof(Port.Id)}\", p.\"{nameof(Port.InterfaceNumber)}\", p.\"{nameof(Port.InterfaceName)}\", p.\"{nameof(Port.InterfaceType)}\", p.\"{nameof(Port.InterfaceStatus)}\", p.\"{nameof(Port.InterfaceSpeed)}\", p.\"{nameof(Port.NetworkDeviceId)}\", p.\"{nameof(Port.ParentPortId)}\",  p.\"{nameof(Port.MACAddress)}\", p.\"{nameof(Port.Description)}\" ");
    sb.Append($"FROM \"{GetTableName<NetworkDevice>()}\" AS nd ");
    sb.Append($"LEFT JOIN \"{GetTableName<Port>()}\" AS p ON p.\"{nameof(Port.NetworkDeviceId)}\" = nd.\"{nameof(NetworkDevice.Id)}\" ");
    sb.Append($"LEFT JOIN \"{GetTableName<MACEntity>()}\" AS mac ON mac.\"{nameof(MACEntity.PortId)}\" = p.\"{nameof(Port.Id)}\" ");
    sb.Append($"WHERE mac.\"{nameof(MACEntity.MACAddress)}\" = @MAC ");
    sb.Append($"AND nd.\"{nameof(NetworkDevice.Host)}\" in ({hostsToDelete})");

    var query = sb.ToString();
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);

    return await connection.QueryAsync<Port>(query, new { MAC = MACAddress });
  }

  async Task<bool> IPortRepository.ExistsPortWithNameAsync(string name,
                                                           CancellationToken cancellationToken)
  {
    StringBuilder sb = new();
    sb.Append("SELECT COUNT(1) ");
    sb.Append($"FROM \"{GetTableName<Port>()}\" ");
    sb.Append($"WHERE \"{nameof(Port.InterfaceName)}\" like '%{name}%'");

    var query = sb.ToString();
    var connection = _postgreSQLDapperContext.CreateConnection();
    return await connection.ExecuteScalarAsync<bool>(query);
  }
}