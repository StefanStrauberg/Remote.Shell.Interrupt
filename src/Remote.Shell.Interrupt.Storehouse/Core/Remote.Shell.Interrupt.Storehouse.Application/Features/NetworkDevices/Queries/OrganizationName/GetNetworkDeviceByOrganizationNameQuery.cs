namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.OrganizationName;

public record GetNetworkDeviceByOrganizationNameQuery(string OrganizationName) : IQuery<CompoundObjectDTO>;

internal class GetNetworkDeviceByOrganizationNameQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
  : IQueryHandler<GetNetworkDeviceByOrganizationNameQuery, CompoundObjectDTO>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<CompoundObjectDTO> IRequestHandler<GetNetworkDeviceByOrganizationNameQuery, CompoundObjectDTO>.Handle(GetNetworkDeviceByOrganizationNameQuery request,
                                                                                                                   CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(request.OrganizationName))
      throw new ArgumentException("Invalid organization name.", nameof(request.OrganizationName));

    var clientsCODByNameQuery = new GetClientsCODByNameQuery(request.OrganizationName);
    var clientsCODByNameQueryHandler = new GetClientsCODByNameQueryHandler(_unitOfWork, _mapper);
    var clients = await ((IRequestHandler<GetClientsCODByNameQuery, IEnumerable<ClientCODDTO>>)clientsCODByNameQueryHandler).Handle(clientsCODByNameQuery,
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
