

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class PortRepository(DapperContext context) : GenericRepository<Port>(context), IPortRepository
{
  async Task<IEnumerable<Port>> IPortRepository.GetAllAggregatedPortsByListAsync(List<Guid> Ids,
                                                                                 CancellationToken cancellationToken)
  {
    var connection = await _context.CreateConnectionAsync(cancellationToken);
    var ids = GetStringIds(Ids);
    var query = "SELECT \"Id\", \"InterfaceNumber\", \"InterfaceName\", \"InterfaceType\", \"InterfaceStatus\", \"InterfaceSpeed\", \"NetworkDeviceId\", \"ParentPortId\",  \"MACAddress\" \"Description\" " +
                "FROM \"Ports\" " +
                $"WHERE \"ParentPortId\" in ({ids})";
    return await connection.QueryAsync<Port>(query);
  }

  async Task<IEnumerable<Port>> IPortRepository.GetAllAggregatedPortsByIdAsync(Guid id,
                                                                             CancellationToken cancellationToken)
  {
    var connection = await _context.CreateConnectionAsync(cancellationToken);
    var query = "SELECT \"Id\", \"InterfaceNumber\", \"InterfaceName\", \"InterfaceType\", \"InterfaceStatus\", \"InterfaceSpeed\", \"NetworkDeviceId\", \"ParentPortId\",  \"MACAddress\" \"Description\" " +
                "FROM \"Ports\" " +
                "WHERE \"ParentPortId\" = @Id";
    return await connection.QueryAsync<Port>(query, new { Id = id });
  }

  async Task<string> IPortRepository.LookingForInterfaceNameByIPAsync(string ipAddress,
                                                                      CancellationToken cancellationToken)
  {
    var connection = await _context.CreateConnectionAsync(cancellationToken);
    var query = "SELECT p.\"InterfaceName\" FROM \"Ports\" AS p " +
                "LEFT JOIN \"TerminatedNetworkEntities\" AS tn ON tn.\"PortId\" = p.\"Id\" " +
                "WHERE tn.\"NetworkAddress\" IS NOT NULL " +
                "AND tn.\"Netmask\" IS NOT NULL " +
                "AND tn.\"NetworkAddress\" <> 0 " +
                "AND tn.\"Netmask\" <> 0 " +
                "AND((tn.\"NetworkAddress\" & tn.\"Netmask\") = ((@IP::inet - '0.0.0.0'::inet) & tn.\"Netmask\"))";
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
}