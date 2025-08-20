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

    AggregatePorts(networkDevices);
    CleanAggregatedPorts(networkDevices);
    FilterPortsByVlanTags(networkDevices, vlanTags);
    FilterVlansByVlanTags(networkDevices, vlanTags);
    networkDevices = FilterDevices(networkDevices);

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

    spec.AddInclude(nd => nd.PortsOfNetworkDevice);
    spec.AddThenInclude<Port, IEnumerable<VLAN>>(port => port.VLANs);

    if (vlanTags is not null && vlanTags.Any())
      spec.AddFilter(x => x.PortsOfNetworkDevice.Any(x => x.VLANs.Any(x => vlanTags.Contains(x.VLANTag))));

    return spec;
  }

  static void AggregatePorts(IEnumerable<NetworkDevice> networkDevices)
  {
    foreach (var device in networkDevices)
    {
      var allPorts = device.PortsOfNetworkDevice;

      // Выбираем родительские порты (без ParentId)
      var parentPorts = allPorts.Where(p => p.ParentId is null)
                                .ToList();

      if (parentPorts.Count == 0)
        continue;

      // Группируем дочерние порты по ParentId
      var childrenByParent = allPorts.Where(p => p.ParentId.HasValue)
                                     .GroupBy(p => p.ParentId!.Value)
                                     .ToDictionary(g => g.Key, g => g.ToList());

      // Назначаем дочерние порты каждому родителю
      foreach (var parent in parentPorts)
        parent.AggregatedPorts = childrenByParent.TryGetValue(parent.Id, out var aggregated) ? aggregated : [];
    }
  }

  static void CleanAggregatedPorts(IEnumerable<NetworkDevice> networkDevices)
  {
    foreach (var device in networkDevices)
    {
      // Собираем все Id агрегированных портов
      var aggregatedIds = device.PortsOfNetworkDevice.Where(p => p.AggregatedPorts is not null &&
                                                                 p.AggregatedPorts.Count > 0)
                                                     .SelectMany(p => p.AggregatedPorts)
                                                     .Select(p => p.Id)
                                                     .ToHashSet();

      // Очищаем список, исключая агрегированные порты
      device.PortsOfNetworkDevice = [.. device.PortsOfNetworkDevice.Where(p => !aggregatedIds.Contains(p.Id))];
    }
  }

  static void FilterPortsByVlanTags(IEnumerable<NetworkDevice> networkDevices, IEnumerable<int> vlanTags)
  {
    var vlanTagSet = vlanTags as HashSet<int> ?? [.. vlanTags];

    foreach (var device in networkDevices)
      device.PortsOfNetworkDevice = [.. device.PortsOfNetworkDevice.Where(port => port.VLANs.Any(vl => vlanTagSet.Contains(vl.VLANTag)))
                                                                   .Where(port => !port.VLANs.Any(vl => vl.VLANTag == 101))
                                                                   .OrderBy(port => port.InterfaceName)];
  }

  static void FilterVlansByVlanTags(IEnumerable<NetworkDevice> networkDevices, IEnumerable<int> vlanTags)
  {
    var vlanTagSet = vlanTags as HashSet<int> ?? [.. vlanTags];

    foreach (var device in networkDevices)
    {
      foreach (var port in device.PortsOfNetworkDevice)
        port.VLANs = [.. port.VLANs.Where(vlan => vlanTagSet.Contains(vlan.VLANTag))];
    }
  }

  static IEnumerable<NetworkDevice> FilterDevices(IEnumerable<NetworkDevice> networkDevices)
    => [.. networkDevices.Where(nd => nd.PortsOfNetworkDevice.Count != 0)];

  CompoundObjectDTO BuildResult(IEnumerable<DetailClientDTO> clients, IEnumerable<NetworkDevice> devices)
    => new()
       {
         Clients = clients,
         NetworkDevices = mapper.Map<IEnumerable<NetworkDeviceDTO>>(devices)
       };
}
