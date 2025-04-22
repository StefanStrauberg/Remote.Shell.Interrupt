namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetByExpression;

public record GetNetworkDevicesByIPQuery(string IpAddress) : IQuery<CompoundObjectDTO>;

internal class GetNetworkDevicesByIPQueryHandler(INetDevUnitOfWork netDevUnitOfWork,
                                                 ILocBillUnitOfWork locBillUnitOfWork,
                                                 IMapper mapper)
  : IQueryHandler<GetNetworkDevicesByIPQuery, CompoundObjectDTO>
{
  async Task<CompoundObjectDTO> IRequestHandler<GetNetworkDevicesByIPQuery, CompoundObjectDTO>.Handle(GetNetworkDevicesByIPQuery request,
                                                                                                    CancellationToken cancellationToken)
  {
    // Проверка формата IP-адреса
    if (!IPAddress.TryParse(request.IpAddress, out var ipToCheck))
      throw new ArgumentException("Invalid IP address format.", nameof(request.IpAddress));

    var interfaceName = await netDevUnitOfWork.Ports
                                         .LookingForInterfaceNameByIPAsync(request.IpAddress,
                                                                           cancellationToken);

    if (!TryGetVlanNumber.Handle(interfaceName,
                                 out int vlanTag))
      return default!;

    var getClientsByVlanTagQueryHandler = new GetClientsByVlanTagQueryHandler(locBillUnitOfWork,
                                                                              mapper);

    var getClientsByVlanTagQuery = new GetClientsByVlanTagQuery(vlanTag);

    var clients = await ((IRequestHandler<GetClientsByVlanTagQuery, IEnumerable<DetailClientDTO>>)getClientsByVlanTagQueryHandler).Handle(getClientsByVlanTagQuery,
                                                                                                                                          cancellationToken);
    
    var vlanTags = clients.SelectMany(x => x.SPRVlans)
                          .Select(x => x.IdVlan);
                          
    List<NetworkDevice> networkDevices = [];

    foreach (var tag in vlanTags)
    {
      networkDevices.AddRange(await netDevUnitOfWork.NetworkDevices
                                                    .GetManyWithChildrenAsync(new RequestParameters
                                                                              {
                                                                                Filters = $"VLANTag=={tag}"
                                                                              },
                                                                              cancellationToken));
    }

    PrepareAndCleanAggregationPorts.Handle(networkDevices);

    var reuslt = new CompoundObjectDTO()
    {
      NetworkDevices = mapper.Map<IEnumerable<NetworkDeviceDTO>>(networkDevices),
      Clients = clients
    };

    return reuslt;
  }
}
