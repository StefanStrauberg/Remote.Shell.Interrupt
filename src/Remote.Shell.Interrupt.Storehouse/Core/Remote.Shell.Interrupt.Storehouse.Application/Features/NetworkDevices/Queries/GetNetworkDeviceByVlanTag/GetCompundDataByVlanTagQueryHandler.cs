namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetNetworkDeviceByVlanTag;

public record GetCompundDataByVlanTagQuery(int VlanTag) : IQuery<CompoundObjectDTO>;

internal class GetCompundDataByVlanTagQueryHandler(INetDevUnitOfWork unitOfWork,
                                                   INetworkDeviceSpecification netDevSpec,
                                                   IQueryFilterParser parser,
                                                   IMapper mapper,
                                                   IQueryHandler<GetClientsByVlanTagQuery, IEnumerable<DetailClientDTO>> clientsHandler)
  : IQueryHandler<GetCompundDataByVlanTagQuery, CompoundObjectDTO>
{
  async Task<CompoundObjectDTO> IRequestHandler<GetCompundDataByVlanTagQuery, CompoundObjectDTO>.Handle(GetCompundDataByVlanTagQuery request,
                                                                                                        CancellationToken cancellationToken)
  {
    ValidateRequest(request);

    var clients = await FetchClients(request.VlanTag, cancellationToken);
    var vlanTags = ExtractVlanTags(clients);

    var networkDevices = await FetchNetworkDevices(vlanTags, cancellationToken);
    //await AggregatePorts(networkDevices, cancellationToken);

    //PrepareAndCleanAggregationPorts(networkDevices);

    return BuildResult(clients, networkDevices);
  }

  static void ValidateRequest(GetCompundDataByVlanTagQuery request)
  {
    if (request.VlanTag <= 0)
      throw new ArgumentException("Invalid VLAN Tag.", nameof(request.VlanTag));
  }

  async Task<IEnumerable<DetailClientDTO>> FetchClients(int vlanTag, CancellationToken cancellationToken)
  {
    var query = new GetClientsByVlanTagQuery(vlanTag);
    var result = await clientsHandler.Handle(query, cancellationToken);

    return result;
  }

  static IEnumerable<int> ExtractVlanTags(IEnumerable<DetailClientDTO> clients)
    => clients.SelectMany(c => c.SPRVlans)
              .Select(v => v.IdVlan)
              .Distinct();

  async Task<IEnumerable<NetworkDevice>> FetchNetworkDevices(IEnumerable<int> vlanTags, CancellationToken cancellationToken)
  {
    var parameters = RequestParametersFactory.Empty();
    var filterExpr = parser.ParseFilters<NetworkDevice>(parameters.Filters);
    var spec = BuildSpecification(netDevSpec, filterExpr, vlanTags);

    return await unitOfWork.NetworkDevices.GetManyWithChildrenAsync(spec, cancellationToken);
  }

  static ISpecification<NetworkDevice> BuildSpecification(INetworkDeviceSpecification baseSpec,
                                                          Expression<Func<NetworkDevice, bool>>? filterExpr,
                                                          IEnumerable<int>? vlanTags = null)
  {
    var spec = baseSpec;

    if (filterExpr is not null)
      spec.AddFilter(filterExpr);

    // spec.AddInclude(x => x.PortsOfNetworkDevice);

    if (vlanTags is not null && vlanTags.Any())
    {
      spec.AddInclude(nd => nd.PortsOfNetworkDevice.Where(p => p.VLANs.Any(vlan => vlanTags.Contains(vlan.VLANTag))));
      spec.AddThenInclude<Port, IEnumerable<VLAN>>(port => port.VLANs.Where(vlan => vlanTags.Contains(vlan.VLANTag)));
    }
    else
    {
      spec.AddInclude(nd => nd.PortsOfNetworkDevice);
      spec.AddThenInclude<Port, IEnumerable<VLAN>>(port => port.VLANs);
    }

    return spec;
  }

  async Task AggregatePorts(IEnumerable<NetworkDevice> networkDevices, CancellationToken cancellationToken)
  {
    foreach (var device in networkDevices)
    {
      var parentPorts = device.PortsOfNetworkDevice.Where(p => p.ParentId is null);

      if (!parentPorts.Any())
        continue;

      var parentIds = parentPorts.Select(port => port.Id);
      var children = await unitOfWork.Ports.GetAllAggregatedPortsByListAsync(parentIds, cancellationToken);
      var childrenByParent = children.Where(child => child.ParentId.HasValue)
                                     .GroupBy(child => child.ParentId!.Value)
                                     .ToDictionary(group => group.Key, group => group.ToList());

      foreach (var port in parentPorts)
        port.AggregatedPorts = childrenByParent.TryGetValue(port.Id, out var aggregated) ? aggregated : [];
    }
  }

  static void PrepareAndCleanAggregationPorts(IEnumerable<NetworkDevice> networkDevices)
  {
    foreach (var device in networkDevices)
    {
      var portLookup = BuildPortLookup(device);
      var aggregatedPortIds = AttachAggregatedPorts(device, portLookup);
      device.PortsOfNetworkDevice = CleanAndSortPorts(device, aggregatedPortIds);
    }
  }

  static Dictionary<Guid, Port> BuildPortLookup(NetworkDevice device)
    => device.PortsOfNetworkDevice.ToDictionary(port => port.Id);

  static HashSet<Guid> AttachAggregatedPorts(NetworkDevice device, Dictionary<Guid, Port> portLookup)
  {
    var aggregatedPortIds = new HashSet<Guid>();

    foreach (var port in device.PortsOfNetworkDevice.Where(port => port.ParentId.HasValue))
    {
      if (port.ParentId is Guid parentId && portLookup.TryGetValue(parentId, out var parent))
      {
        parent.AggregatedPorts.Add(port);
        aggregatedPortIds.Add(port.Id);
      }
    }

    return aggregatedPortIds;
  }

  static List<Port> CleanAndSortPorts(NetworkDevice device, HashSet<Guid> aggregatedPortIds)
    => [.. device.PortsOfNetworkDevice
                 .Where(port => !aggregatedPortIds.Contains(port.Id))
                 .OrderBy(port => port.InterfaceName)];

  CompoundObjectDTO BuildResult(IEnumerable<DetailClientDTO> clients, IEnumerable<NetworkDevice> devices)
    => new()
       {
         Clients = clients,
         NetworkDevices = mapper.Map<IEnumerable<NetworkDeviceDTO>>(devices)
       };
}
