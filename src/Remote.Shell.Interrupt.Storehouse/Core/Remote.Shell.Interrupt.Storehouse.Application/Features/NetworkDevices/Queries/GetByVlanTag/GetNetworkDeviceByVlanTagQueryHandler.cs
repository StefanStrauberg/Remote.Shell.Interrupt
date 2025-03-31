namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetByVlanTag;

public record GetNetworkDeviceByVlanTagQuery(int VLANTag) : IQuery<CompoundObjectDTO>;

internal class GetNetworkDeviceByVlanTagQueryHandler(IUnitOfWork unitOfWork,
                                                     IMapper mapper)
  : IQueryHandler<GetNetworkDeviceByVlanTagQuery, CompoundObjectDTO>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<CompoundObjectDTO> IRequestHandler<GetNetworkDeviceByVlanTagQuery, CompoundObjectDTO>.Handle(GetNetworkDeviceByVlanTagQuery request,
                                                                                                        CancellationToken cancellationToken)
  {
    if (request.VLANTag == 0)
      throw new ArgumentException("Invalid VLAN Tag.", nameof(request.VLANTag));

    var clientsCODByVlanTagQueryHandler = new GetClientsCODByVlanTagQueryHandler(_unitOfWork,
                                                                                 _mapper);
    var clientsCODByVlanTagQuery = new GetClientsCODByVlanTagQuery(request.VLANTag);
    var clients = await ((IRequestHandler<GetClientsCODByVlanTagQuery, IEnumerable<ClientCODDTODetail>>)clientsCODByVlanTagQueryHandler).Handle(clientsCODByVlanTagQuery,
                                                                                                                                                cancellationToken);
    List<int> vlanTags = [.. clients.SelectMany(x => x.SPRVlans).Select(x => x.IdVlan)];
    List<NetworkDevice> networkDevices = [];

    foreach (var tag in vlanTags)
    {
      networkDevices.AddRange(await _unitOfWork.NetworkDevices
                                               .GetAllWithChildrensByVLANTagAsync(tag,
                                                                                  cancellationToken));
    }

    PrepareAndCleanAggregationPorts(networkDevices);

    var reuslt = new CompoundObjectDTO()
    {
      NetworkDevices = _mapper.Map<IEnumerable<NetworkDeviceDTO>>(networkDevices),
      ClientCODs = clients
    };

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
