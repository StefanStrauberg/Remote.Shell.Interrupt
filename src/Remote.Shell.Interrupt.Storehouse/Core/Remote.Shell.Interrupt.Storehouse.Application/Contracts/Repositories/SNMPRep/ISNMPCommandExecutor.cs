namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.SNMPRep;

/// <summary>
/// Defines an interface for executing SNMP commands, providing methods for retrieving SNMP data.
/// </summary>
public interface ISNMPCommandExecutor
{
    /// <summary>
    /// Executes an SNMP walk command to retrieve multiple sequential values from the specified host.
    /// </summary>
    /// <param name="host">The target device's hostname or IP address.</param>
    /// <param name="community">The SNMP community string for authentication.</param>
    /// <param name="oid">The Object Identifier (OID) to start the SNMP walk.</param>
    /// <param name="cancellationToken">A token for canceling the operation if needed.</param>
    /// <param name="toHex">Determines whether results should be converted to hexadecimal format.</param>
    /// <param name="repetitions">The number of repetitions to perform within the walk operation.</param>
    /// <returns>A list of SNMP response objects containing retrieved data.</returns>
    Task<List<SNMPResponse>> WalkCommand(string host,
                                         string community,
                                         string oid,
                                         CancellationToken cancellationToken,
                                         bool toHex = false,
                                         int repetitions = 20);

    /// <summary>
    /// Executes an SNMP get command to retrieve a single value from the specified host.
    /// </summary>
    /// <param name="host">The target device's hostname or IP address.</param>
    /// <param name="community">The SNMP community string for authentication.</param>
    /// <param name="oid">The Object Identifier (OID) specifying the target value.</param>
    /// <param name="cancellationToken">A token for canceling the operation if needed.</param>
    /// <param name="toHex">Determines whether results should be converted to hexadecimal format.</param>
    /// <returns>An SNMP response object containing the retrieved value.</returns>
    Task<SNMPResponse> GetCommand(string host,
                                  string community,
                                  string oid,
                                  CancellationToken cancellationToken,
                                  bool toHex = false);
}
