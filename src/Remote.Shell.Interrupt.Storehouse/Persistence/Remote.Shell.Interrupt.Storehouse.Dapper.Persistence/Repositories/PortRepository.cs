namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class PortRepository(DapperContext context) : GenericRepository<Port>(context), IPortRepository
{
  public async Task<string> LookingForInterfaceNameByIP(string ipAddress,
                                                        CancellationToken cancellationToken)
  {
    var connection = await _context.CreateConnectionAsync(cancellationToken);
    var query = "select p.\"InterfaceName\" from \"Ports\" as p " +
                "left join \"TerminatedNetworkEntities\" as tn on tn.\"PortId\" = p.\"Id\" " +
                "where tn.\"NetworkAddress\" is not null " +
                "and tn.\"Netmask\" is not null " +
                "and tn.\"NetworkAddress\" <> 0 " +
                "and tn.\"Netmask\" <> 0 " +
                "and((tn.\"NetworkAddress\" & tn.\"Netmask\") = ((@IP::inet - '0.0.0.0'::inet) & tn.\"Netmask\"))";
    var reuslt = await connection.ExecuteScalarAsync<string>(query, new { IP = ipAddress });
    return reuslt ?? string.Empty;
  }
}