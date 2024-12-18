
namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetByVlanTag;

public record GetNetworkDeviceByVlanTagQuery(int VLANTag) : IQuery<IEnumerable<NetworkDeviceDTO>>;

internal class GetNetworkDeviceByVlanTagQueryHandler(IUnitOfWork unitOfWork,
                                                     IMapper mapper)
  : IQueryHandler<GetNetworkDeviceByVlanTagQuery, IEnumerable<NetworkDeviceDTO>>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<IEnumerable<NetworkDeviceDTO>> IRequestHandler<GetNetworkDeviceByVlanTagQuery, IEnumerable<NetworkDeviceDTO>>.Handle(GetNetworkDeviceByVlanTagQuery request,
                                                                                                                                  CancellationToken cancellationToken)
  {
    if (request.VLANTag == 0)
      return [];

    var networkDevices = await _unitOfWork.NetworkDevices
                                          .GetAllWithChildrensByVLANTagAsync(request.VLANTag,
                                                                             cancellationToken);

    PrepareAndCleanAggregationPorts(networkDevices);

    var existsDeletePortBase = await _unitOfWork.Ports
                                                .ExistsPortWithNameAsync(request.VLANTag.ToString(),
                                                                         cancellationToken);

    if (existsDeletePortBase)
    {
      var deletePortBase = await _unitOfWork.Ports
                                            .GetPortWithNameAsync(request.VLANTag.ToString(),
                                                                  cancellationToken);

      if (deletePortBase is not null)
      {
        networkDevices = networkDevices.Where(x => x.Id != deletePortBase.NetworkDeviceId);

        var portsToDelete = await _unitOfWork.Ports.GetPortsWithWithMacAddressesAndSpecificHostsAsync(deletePortBase.MACAddress,
                                                                                                      networkDevices.Select(x => x.Host)
                                                                                                                    .ToList(),
                                                                                                      cancellationToken);

        foreach (var port in portsToDelete)
        {
          var nd = networkDevices.Where(x => x.Id == port.NetworkDeviceId)
                                 .FirstOrDefault();
          if (nd is null)
            continue;

          var portToDelete = nd.PortsOfNetworkDevice
                               .Where(x => x.Id == port.Id)
                               .FirstOrDefault();

          if (portToDelete is null)
            continue;

          nd.PortsOfNetworkDevice.Remove(portToDelete);

          if (nd.PortsOfNetworkDevice.Count == 0)
          {
            networkDevices = networkDevices.Where(x => x.Id != nd.Id);
          }
        }
      }
    }

    var portIdsToQuery = networkDevices.SelectMany(nd => nd.PortsOfNetworkDevice)
                                       .Where(port => port.AggregatedPorts.Count == 0)
                                       .Select(port => port.Id)
                                       .Distinct()
                                       .ToList();

    if (portIdsToQuery.Count > 0)
    {
      var aggregatedPorts = await _unitOfWork.Ports
                                             .GetAllAggregatedPortsByListAsync(portIdsToQuery, cancellationToken);

      var aggregatedPortsDict = aggregatedPorts.GroupBy(x => x.ParentPortId!.Value)
                                               .ToDictionary(x => x.Key, x => x.ToList());

      foreach (var aggPort in aggregatedPortsDict)
      {
        var foundPort = networkDevices.SelectMany(nd => nd.PortsOfNetworkDevice)
                                      .First(port => port.Id == aggPort.Key);
        foundPort.AggregatedPorts = aggPort.Value;
      }
    }

    var reuslt = _mapper.Map<IEnumerable<NetworkDeviceDTO>>(networkDevices);
    return reuslt;
  }

  static void PrepareAndCleanAggregationPorts(IEnumerable<NetworkDevice> networkDevices)
  {
    foreach (var networkDevice in networkDevices)
    {
      HashSet<Guid> aggregatedPortsIds = [];

      foreach (var port in networkDevice.PortsOfNetworkDevice.Where(x => x.ParentPortId is not null))
      {
        var parentPort = networkDevice.PortsOfNetworkDevice.First(x => x.Id == port.ParentPortId);
        parentPort.AggregatedPorts.Add(port);
        aggregatedPortsIds.Add(port.Id);
      }

      // Фильтруем PortsOfNetworkDevice, исключая повторяющиеся порты
      networkDevice.PortsOfNetworkDevice = [.. networkDevice.PortsOfNetworkDevice
          .Where(port => !aggregatedPortsIds.Contains(port.Id))
          .OrderBy(port => port.InterfaceName)];
    }
  }
}
