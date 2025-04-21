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

    if (!TryGetVlanNumber.Handle(interfaceName,
                                 out int vlanTag))
      return default!;

    var getClientsByVlanTagQueryHandler = new GetClientsByVlanTagQueryHandler(_unitOfWork,
                                                                              _mapper);

    var getClientsByVlanTagQuery = new GetClientsByVlanTagQuery(vlanTag);

    var clients = await ((IRequestHandler<GetClientsByVlanTagQuery, IEnumerable<DetailClientDTO>>)getClientsByVlanTagQueryHandler).Handle(getClientsByVlanTagQuery,
                                                                                                                                          cancellationToken);
    
    var vlanTags = clients.SelectMany(x => x.SPRVlans)
                          .Select(x => x.IdVlan);
                          
    List<NetworkDevice> networkDevices = [];

    foreach (var tag in vlanTags)
    {
      networkDevices.AddRange(await _unitOfWork.NetworkDevices
                                               .GetManyWithChildrensByVLANTagAsync(tag,
                                                                                   cancellationToken));
    }

    PrepareAndCleanAggregationPorts.Handle(networkDevices);

    var reuslt = new CompoundObjectDTO()
    {
      NetworkDevices = _mapper.Map<IEnumerable<NetworkDeviceDTO>>(networkDevices),
      Clients = clients
    };

    return reuslt;
  }
}
