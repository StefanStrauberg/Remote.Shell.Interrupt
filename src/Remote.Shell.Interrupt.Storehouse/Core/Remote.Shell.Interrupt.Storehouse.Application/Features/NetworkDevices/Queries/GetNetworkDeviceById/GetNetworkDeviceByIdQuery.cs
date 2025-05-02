namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetNetworkDeviceById;

/// <summary>
/// Represents a query to retrieve a network device by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the network device to retrieve.</param>
public record GetNetworkDeviceByIdQuery(Guid Id) : IQuery<NetworkDeviceDTO>;

/// <summary>
/// Handles the GetNetworkDeviceByIdQuery and retrieves the specified network device.
/// </summary>
/// <remarks>
/// This handler applies filtering criteria, verifies the device's existence, 
/// retrieves related data, processes port aggregation, and returns the mapped result.
/// </remarks>
/// <param name="netDevUnitOfWork">Unit of work for network device-related database operations.</param>
/// <param name="specification">Specification used for filtering network devices.</param>
/// <param name="queryFilterParser">Parser for processing filter expressions.</param>
/// <param name="mapper">Object mapper for DTO transformation.</param>
internal class GetNetworkDeviceByIdQueryHandler(INetDevUnitOfWork netDevUnitOfWork,
                                                INetworkDeviceSpecification specification,
                                                IQueryFilterParser queryFilterParser,
                                                IMapper mapper)
  : IQueryHandler<GetNetworkDeviceByIdQuery, NetworkDeviceDTO>
{
  /// <summary>
  /// Handles the request to retrieve a network device by its ID.
  /// </summary>
  /// <param name="request">The query containing the network device ID.</param>
  /// <param name="cancellationToken">Token to handle request cancellation.</param>
  /// <returns>A DTO representing the network device.</returns>
  async Task<NetworkDeviceDTO> IRequestHandler<GetNetworkDeviceByIdQuery, NetworkDeviceDTO>.Handle(GetNetworkDeviceByIdQuery request,
                                                                                                   CancellationToken cancellationToken)
  {
    // Create filtering parameters based on the provided network device ID
    var requestParameters = new RequestParameters
    {
      Filters = [
        new ()
        {
          PropertyPath = "Id",
          Operator = FilterOperator.Equals,
          Value = request.Id.ToString()
        }
      ]
    };

    // Parse filter expression
    var filterExpr = queryFilterParser.ParseFilters<NetworkDevice>(requestParameters.Filters);

    // Build base specification with filtering applied
    var baseSpec = BuildSpecification(specification,
                                      filterExpr);

    // Check if the network device exists
    var existing = await netDevUnitOfWork.NetworkDevices
                                         .AnyByQueryAsync(baseSpec,
                                                          cancellationToken);

    // If the network device is not found, throw an exception
    if (!existing)
      throw new EntityNotFoundException(typeof(NetworkDevice),
                                        filterExpr is not null ? filterExpr.ToString() : string.Empty);

    // Retrieve the network device along with its child entities
    var networkDevice = await netDevUnitOfWork.NetworkDevices
                                              .GetOneWithChildrenAsync(baseSpec,
                                                                        cancellationToken);

    // Process port aggregation by linking child ports to their parent ports
    HashSet<Guid> aggregatedPortsIds = [];

    foreach (var port in networkDevice.PortsOfNetworkDevice.Where(x => x.ParentPortId is not null))
    {
      var parentPort = networkDevice.PortsOfNetworkDevice.First(x => x.Id == port.ParentPortId);
      parentPort.AggregatedPorts.Add(port);
      aggregatedPortsIds.Add(port.Id);
    }

    // Filter PortsOfNetworkDevice, excluding duplicate aggregated ports
    networkDevice.PortsOfNetworkDevice = [.. networkDevice.PortsOfNetworkDevice
          .Where(port => !aggregatedPortsIds.Contains(port.Id))
          .OrderBy(port => port.InterfaceName)];

    // Map the network device entity to a DTO
    var networkDeviceDTO = mapper.Map<NetworkDeviceDTO>(networkDevice);

    // Return the mapped result
    return networkDeviceDTO;
  }

  /// <summary>
  /// Builds the specification by applying filtering and includes related entities.
  /// </summary>
  /// <param name="baseSpec">The base network device specification.</param>
  /// <param name="filterExpr">The filter expression to apply.</param>
  /// <returns>An updated specification with filtering applied.</returns>
  static INetworkDeviceSpecification BuildSpecification(INetworkDeviceSpecification baseSpec,
                                                        Expression<Func<NetworkDevice, bool>>? filterExpr)
  {
    var spec = baseSpec.AddInclude(x => x.PortsOfNetworkDevice);

    if (filterExpr is not null)
        spec.AddFilter(filterExpr);

    return (INetworkDeviceSpecification)spec;
  }
}
