namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetNetworkDeviceById;

/// <summary>
/// Query for retrieving a <see cref="NetworkDeviceDTO"/> by its unique identifier.
/// </summary>
public record GetNetworkDeviceByIdQuery(Guid Id)
  : FindEntityByFilterQuery<NetworkDeviceDTO>(RequestParameters.ForId(Id));

/// <summary>
/// Handler for processing <see cref="GetNetworkDeviceByIdQuery"/> requests.
/// Applies filtering, includes port relationships, and transforms the result into a DTO.
/// </summary>
/// <param name="netDevUnitOfWork">Unit of work for network device operations.</param>
/// <param name="specification">Base specification for querying network devices.</param>
/// <param name="queryFilterParser">Parser for translating request filters into expressions.</param>
/// <param name="mapper">Mapper for converting entities to DTOs.</param>
internal class GetNetworkDeviceByIdQueryHandler(INetDevUnitOfWork netDevUnitOfWork,
                                                INetworkDeviceSpecification specification,
                                                IQueryFilterParser queryFilterParser,
                                                IMapper mapper)
  : FindEntityByFilterQueryHandler<NetworkDevice, NetworkDeviceDTO, GetNetworkDeviceByIdQuery>(specification, queryFilterParser, mapper)
{
  /// <summary>
  /// Local reference to the query filter parser.
  /// </summary>
  readonly IQueryFilterParser _queryFilterParser = queryFilterParser;

  /// <summary>
  /// Handles the query by building a specification, validating existence, fetching the entity,
  /// processing port relationships, and mapping to a DTO.
  /// </summary>
  /// <param name="request">The query containing the entity ID.</param>
  /// <param name="cancellationToken">Token for cancelling the operation.</param>
  /// <returns>The mapped <see cref="NetworkDeviceDTO"/>.</returns>
  public override async Task<NetworkDeviceDTO> Handle(GetNetworkDeviceByIdQuery request, CancellationToken cancellationToken)
  {
    var specification = BuildSpecification(request.Parameters);

    await EnsureEntityExistAsync(specification, cancellationToken);

    var networkDevice = await FetchEntityAsync(specification, cancellationToken);

    ProcessPorts(networkDevice);

    return MapToDto(networkDevice);
  }

  /// <summary>
  /// Processes port relationships by aggregating child ports and filtering out duplicates.
  /// </summary>
  /// <param name="networkDevice">The network device whose ports are being processed.</param>
  static void ProcessPorts(NetworkDevice networkDevice)
  {
    var aggregatedPortsIds = new HashSet<Guid>();

    AggregateChildPorts(networkDevice, aggregatedPortsIds);
    FilterUniquePorts(networkDevice, aggregatedPortsIds);
  }

  /// <summary>
  /// Filters out ports that have been aggregated into parent ports.
  /// </summary>
  /// <param name="networkDevice">The network device being filtered.</param>
  /// <param name="aggregatedPortsIds">Set of aggregated port IDs to exclude.</param>
  static void FilterUniquePorts(NetworkDevice networkDevice, HashSet<Guid> aggregatedPortsIds)
  {
    networkDevice.PortsOfNetworkDevice = [.. networkDevice.PortsOfNetworkDevice
                                                          .Where(port => !aggregatedPortsIds.Contains(port.Id))
                                                          .OrderBy(port => port.InterfaceName)];
  }

  /// <summary>
  /// Aggregates child ports into their respective parent ports.
  /// </summary>
  /// <param name="networkDevice">The network device whose ports are being aggregated.</param>
  /// <param name="aggregatedPortsIds">Set to track aggregated port IDs.</param>
  static void AggregateChildPorts(NetworkDevice networkDevice, HashSet<Guid> aggregatedPortsIds)
  {
    foreach (var port in networkDevice.PortsOfNetworkDevice.Where(x => x.ParentId is not null))
    {
      var parentPort = networkDevice.PortsOfNetworkDevice.First(x => x.Id == port.ParentId);
      parentPort.AggregatedPorts.Add(port);
      aggregatedPortsIds.Add(port.Id);
    }
  }

  /// <summary>
  /// Builds a specification for querying the network device, including port relationships.
  /// </summary>
  /// <param name="requestParameters">The request parameters containing filters.</param>
  /// <returns>A specification with filters and includes applied.</returns>
  protected override ISpecification<NetworkDevice> BuildSpecification(RequestParameters requestParameters)
  {
    var filterExpr = _queryFilterParser.ParseFilters<NetworkDevice>(requestParameters.Filters);
    var spec = specification.AddInclude(x => x.PortsOfNetworkDevice);

    if (filterExpr is not null)
      spec.AddFilter(filterExpr);

    return spec;
  }

  /// <summary>
  /// Validates that the network device exists based on the specification.
  /// </summary>
  /// <param name="specification">Specification used to locate the device.</param>
  /// <param name="cancellationToken">Token for cancelling the operation.</param>
  protected override async Task EnsureEntityExistAsync(ISpecification<NetworkDevice> specification,
                                                       CancellationToken cancellationToken)
    => await netDevUnitOfWork.NetworkDevices.AnyByQueryAsync(specification, cancellationToken);

  /// <summary>
  /// Fetches the network device along with its child entities.
  /// </summary>
  /// <param name="specification">Specification used to locate the device.</param>
  /// <param name="cancellationToken">Token for cancelling the operation.</param>
  /// <returns>The fetched <see cref="NetworkDevice"/> entity.</returns>
  protected override async Task<NetworkDevice> FetchEntityAsync(ISpecification<NetworkDevice> specification,
                                                                CancellationToken cancellationToken)
    => await netDevUnitOfWork.NetworkDevices.GetOneWithChildrenAsync(specification, cancellationToken);
}
