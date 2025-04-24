namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.NetDevRep;

public interface IPortRepository
  : IExistenceQueryRepository<Port>,
    IOneQueryRepository<Port>,
    IBulkInsertRepository<Port>,
    IBulkDeleteRepository<Port>,
    IBulkReplaceRepository<Port>
{
  Task<IEnumerable<Port>> GetAllAggregatedPortsByListAsync(IEnumerable<Guid> Ids,
                                                           CancellationToken cancellationToken);
  Task<string> LookingForInterfaceNameByIPAsync(string ipAddress,
                                                CancellationToken cancellationToken);
  Task<IEnumerable<Port>> GetPortsWithMacAddressesAndSpecificHostsAsync(string MACAddress,
                                                                        List<string> hosts,
                                                                        CancellationToken cancellationToken);
}