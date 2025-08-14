namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetNetworkDeviceByVlanTag;

public record GetCompundDataByVlanTagQuery(int VlanTag) : IQuery<CompoundObjectDTO>;

internal class GetCompundDataByVlanTagQueryHandler(INetDevUnitOfWork netDevUnitOfWork,
                                                   ILocBillUnitOfWork locBillUnitOfWork,
                                                   INetworkDeviceSpecification networkDeviceSpecification,
                                                   ISPRVlanSpecification sPRVlanSpecification,
                                                   IClientSpecification clientSpecification,
                                                   IQueryFilterParser queryFilterParser,
                                                   IMapper mapper)
  : IQueryHandler<GetCompundDataByVlanTagQuery, CompoundObjectDTO>
{
  async Task<CompoundObjectDTO> IRequestHandler<GetCompundDataByVlanTagQuery, CompoundObjectDTO>.Handle(GetCompundDataByVlanTagQuery request,
                                                                                                          CancellationToken cancellationToken)
  {
    // Validate VLAN tag
    if (request.VlanTag == 0)
      throw new ArgumentException("Invalid VLAN Tag.", nameof(request.VlanTag));

    // Create a query for retrieving clients associated with the VLAN tag
    var getClientsByVlanTagQuery = new GetClientsByVlanTagQuery(request.VlanTag);

    // Initialize the handler for retrieving clients
    var getClientsByVlanTagQueryHandler = new GetClientsByVlanTagQueryHandler(locBillUnitOfWork,
                                                                              sPRVlanSpecification,
                                                                              clientSpecification,
                                                                              queryFilterParser,
                                                                              mapper);

    // Retrieve the list of clients associated with the VLAN tag
    var clients = await ((IRequestHandler<GetClientsByVlanTagQuery, IEnumerable<DetailClientDTO>>)getClientsByVlanTagQueryHandler).Handle(getClientsByVlanTagQuery,
                                                                                                                                          cancellationToken);
  

    // Extract distinct VLAN tags from the clients' associated VLANs
    var vlanTags = clients.SelectMany(x => x.SPRVlans)
                          .Select(x => x.IdVlan)
                          .Distinct();

    // Create filtering parameters to retrieve network devices by VLAN tag
    var requestParameters = new RequestParameters
    {
      Filters = [
        new ()
        {
          PropertyPath = $"{nameof(NetworkDevice.PortsOfNetworkDevice)}.{nameof(Port.VLANs)}.{nameof(VLAN.VLANTag)}",
          Operator = FilterOperator.In,
          Value = Converter.ArrayToString<int>(vlanTags),
        }
      ]
    };

    // Parse the filter expression
    var filterExpr = queryFilterParser.ParseFilters<NetworkDevice>(requestParameters.Filters);

    // Build the base specification with filtering applied
    var baseSpec = BuildSpecification(networkDeviceSpecification,
                                      filterExpr);

    // Retrieve devices associated with VLAN tags
    var networkDevices = await netDevUnitOfWork.NetworkDevices
                                               .GetManyWithChildrenAsync(baseSpec,
                                                                         cancellationToken);

    // Process aggregated ports for network devices
    foreach (var networkDevice in networkDevices)
    {
      var parentPorts = networkDevice.PortsOfNetworkDevice
                                     .Where(port => port.ParentId is null);

      // Skip devices without parent ports
      if (!parentPorts.Any())
          continue;

      var parentPortsIds = parentPorts.Select(port => port.Id);

      // Retrieve all aggregated child ports for parent ports
      var children = await netDevUnitOfWork.Ports
                                           .GetAllAggregatedPortsByListAsync(parentPortsIds,
                                                                             cancellationToken);

      // Group child ports by their parent port ID
      var childrenByParent = children.Where(child => child.ParentId.HasValue)
                                     .GroupBy(child => child.ParentId!.Value)
                                     .ToDictionary(group => group.Key, 
                                                   group => group.ToList());

      // Assign aggregated child ports to their respective parent ports
      foreach (var port in parentPorts)
          port.AggregatedPorts = childrenByParent.TryGetValue(port.Id, out var aggregated) ? aggregated
                                                                                           : [];
    }

    // Clean and prepare aggregated ports for processing
    PrepareAndCleanAggregationPorts.Handle(networkDevices);

    // Construct the final result DTO
    var result = new CompoundObjectDTO()
    {
      NetworkDevices = mapper.Map<IEnumerable<NetworkDeviceDTO>>(networkDevices),
      Clients = clients
    };

    // Return the result
    return result;
  }

  static INetworkDeviceSpecification BuildSpecification(INetworkDeviceSpecification baseSpec,
                                                        Expression<Func<NetworkDevice, bool>>? filterExpr)
  {
    var spec = baseSpec.AddInclude(x => x.PortsOfNetworkDevice)
                       .AddThenInclude<Port, IEnumerable<VLAN>>(p => p.VLANs);

    if (filterExpr is not null)
      spec.AddFilter(filterExpr);

    return (INetworkDeviceSpecification)spec;
  }
}
