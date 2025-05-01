namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetNetworkDeviceById;

public record GetNetworkDeviceByIdQuery(Guid Id) : IQuery<NetworkDeviceDTO>;

internal class GetNetworkDeviceByIdQueryHandler(INetDevUnitOfWork netDevUnitOfWork,
                                                INetworkDeviceSpecification specification,
                                                IQueryFilterParser queryFilterParser,
                                                IMapper mapper)
  : IQueryHandler<GetNetworkDeviceByIdQuery, NetworkDeviceDTO>
{
  async Task<NetworkDeviceDTO> IRequestHandler<GetNetworkDeviceByIdQuery, NetworkDeviceDTO>.Handle(GetNetworkDeviceByIdQuery request,
                                                                                                   CancellationToken cancellationToken)
  {
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

    // Parse filter
    var filterExpr = queryFilterParser.ParseFilters<NetworkDevice>(requestParameters.Filters);

    // Build base specification
    var baseSpec = BuildSpecification(specification,
                                      filterExpr);

    // Проверка существования сетевого устройства с ID
    var existing = await netDevUnitOfWork.NetworkDevices
                                         .AnyByQueryAsync(baseSpec,
                                                          cancellationToken);

    // Если сетевое устройство не найден — исключение
    if (!existing)
      throw new EntityNotFoundException(typeof(NetworkDevice),
                                        filterExpr is not null ? filterExpr.ToString() : string.Empty);

    var networkDevice = await netDevUnitOfWork.NetworkDevices
                                              .GetOneWithChildrenAsync(baseSpec,
                                                                        cancellationToken);

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

    var networkDeviceDTO = mapper.Map<NetworkDeviceDTO>(networkDevice);

    return networkDeviceDTO;
  }

  static INetworkDeviceSpecification BuildSpecification(INetworkDeviceSpecification baseSpec,
                                                        Expression<Func<NetworkDevice, bool>>? filterExpr)
  {
    var spec = baseSpec.AddInclude(x => x.PortsOfNetworkDevice);

    if (filterExpr is not null)
        spec.AddFilter(filterExpr);

    return (INetworkDeviceSpecification)spec;
  }
}
