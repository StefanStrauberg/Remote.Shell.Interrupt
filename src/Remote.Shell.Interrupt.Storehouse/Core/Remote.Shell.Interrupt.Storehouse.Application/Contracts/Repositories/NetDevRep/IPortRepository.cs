namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.NetDevRep;

/// <summary>
/// Defines a repository interface for managing Port entities, supporting various query and persistence operations.
/// </summary>
public interface IPortRepository
  : IExistenceQueryRepository<Port>,
    IOneQueryRepository<Port>,
    IBulkInsertRepository<Port>,
    IBulkDeleteRepository<Port>,
    IBulkReplaceRepository<Port>
{
  /// <summary>
  /// Asynchronously retrieves aggregated port data based on a list of port identifiers.
  /// </summary>
  /// <param name="Ids">A collection of port GUIDs to filter the aggregation.</param>
  /// <param name="cancellationToken">A token for canceling the operation if needed.</param>
  /// <returns>An enumerable collection of aggregated port entities.</returns>
  Task<IEnumerable<Port>> GetAllAggregatedPortsByListAsync(IEnumerable<Guid> Ids,
                                                           CancellationToken cancellationToken);

  /// <summary>
  /// Asynchronously finds the network interface name associated with a given IP address.
  /// </summary>
  /// <param name="ipAddress">The IP address used to locate the corresponding network interface.</param>
  /// <param name="cancellationToken">A token for canceling the operation if needed.</param>
  /// <returns>The name of the network interface associated with the specified IP address.</returns>
  Task<string> LookingForInterfaceNameByIPAsync(string ipAddress,
                                                CancellationToken cancellationToken);

  /// <summary>
  /// Asynchronously retrieves port entities linked to a specific MAC address and a given list of hosts.
  /// </summary>
  /// <param name="MACAddress">The MAC address used for filtering the ports.</param>
  /// <param name="hosts">A list of host names associated with the ports.</param>
  /// <param name="cancellationToken">A token for canceling the operation if needed.</param>
  /// <returns>An enumerable collection of ports associated with the specified MAC address and host list.</returns>
  Task<IEnumerable<Port>> GetPortsWithMacAddressesAndSpecificHostsAsync(string MACAddress,
                                                                        List<string> hosts,
                                                                        CancellationToken cancellationToken);
}