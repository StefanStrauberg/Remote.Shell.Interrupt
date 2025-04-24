namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetByVlanTag;

public record GetNetworkDeviceByVlanTagQuery(int VlanTag) : IQuery<CompoundObjectDTO>;

internal class GetNetworkDeviceByVlanTagQueryHandler(INetDevUnitOfWork netDevUnitOfWork,
                                                     ILocBillUnitOfWork locBillUnitOfWork,
                                                     IMapper mapper)
  : IQueryHandler<GetNetworkDeviceByVlanTagQuery, CompoundObjectDTO>
{
  async Task<CompoundObjectDTO> IRequestHandler<GetNetworkDeviceByVlanTagQuery, CompoundObjectDTO>.Handle(GetNetworkDeviceByVlanTagQuery request,
                                                                                                          CancellationToken cancellationToken)
  {
    if (request.VlanTag == 0)
      throw new ArgumentException("Invalid VLAN Tag.", nameof(request.VlanTag));

    var getClientsByVlanTagQuery = new GetClientsByVlanTagQuery(request.VlanTag);

    var getClientsByVlanTagQueryHandler = new GetClientsByVlanTagQueryHandler(locBillUnitOfWork,
                                                                              mapper);

    var clients = await ((IRequestHandler<GetClientsByVlanTagQuery, IEnumerable<DetailClientDTO>>)getClientsByVlanTagQueryHandler).Handle(getClientsByVlanTagQuery,
                                                                                                                                          cancellationToken);
    
    List<NetworkDevice> networkDevices = [];

    var vlanTags = clients.SelectMany(x => x.SPRVlans)
                          .Select(x => x.IdVlan);

    foreach (var tag in vlanTags)
    {
      networkDevices.AddRange(await netDevUnitOfWork.NetworkDevices
                                                    .GetManyWithChildrenByVlanTagAsync(request.VlanTag,
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
