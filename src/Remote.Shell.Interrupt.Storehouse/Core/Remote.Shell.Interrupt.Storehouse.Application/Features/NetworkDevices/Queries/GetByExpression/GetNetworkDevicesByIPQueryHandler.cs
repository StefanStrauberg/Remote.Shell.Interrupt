namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetByExpression;

public record GetNetworkDevicesByIPQuery(string IpAddress) : IQuery<CompoundObjectDTO>;

internal class GetNetworkDevicesByIPQueryHandler(IUnitOfWork unitOfWork,
                                                 IMapper mapper)
  : IQueryHandler<GetNetworkDevicesByIPQuery, CompoundObjectDTO>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<CompoundObjectDTO> IRequestHandler<GetNetworkDevicesByIPQuery, CompoundObjectDTO>.Handle(GetNetworkDevicesByIPQuery request,
                                                                                                    CancellationToken cancellationToken)
  {
    // Проверка формата IP-адреса
    if (!IPAddress.TryParse(request.IpAddress, out var ipToCheck))
      throw new ArgumentException("Invalid IP address format.", nameof(request.IpAddress));

    var interfaceName = await _unitOfWork.Ports
                                         .LookingForInterfaceNameByIPAsync(request.IpAddress,
                                                                           cancellationToken);

    var vlanTag = TryGetVlanNumber(interfaceName);

    if (vlanTag == 0)
      return null!;

    var getClientByVlanTagQueryHandler = new GetClientByVlanTagQueryHandler(_unitOfWork, _mapper);
    var getClientByVlanTagQuery = new GetClientByVlanTagQuery(vlanTag);
    var client = await ((IRequestHandler<GetClientByVlanTagQuery, DetailClientDTO>)getClientByVlanTagQueryHandler).Handle(getClientByVlanTagQuery,
                                                                                                                          cancellationToken);
    
    var vlanTags = client.SPRVlans
                         .Select(x => x.IdVlan);
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
      Client = client
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

  static int TryGetVlanNumber(string interfaceName)
  {
    StringBuilder result = new();

    foreach (char c in interfaceName)
    {
      if (char.IsDigit(c))  // Проверяем, является ли символ цифрой
      {
        result.Append(c);  // Добавляем цифру в результат
      }
    }

    if (int.TryParse(result.ToString(), out int vlanTag))
      return vlanTag;
    else
      return 0;
  }
}
