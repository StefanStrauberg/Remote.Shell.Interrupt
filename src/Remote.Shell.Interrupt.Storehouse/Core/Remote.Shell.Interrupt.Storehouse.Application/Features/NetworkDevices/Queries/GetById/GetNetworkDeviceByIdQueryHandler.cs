
namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetById;

internal class GetNetworkDeviceByIdQueryHandler(IUnitOfWork unitOfWork,
                                                IMapper mapper)
  : IQueryHandler<GetNetworkDeviceByIdQuery, NetworkDeviceDTO>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  public async Task<NetworkDeviceDTO> Handle(GetNetworkDeviceByIdQuery request, CancellationToken cancellationToken)
  {
    var requestExpression = (Expression<Func<NetworkDevice, bool>>)(x => x.Id == request.Id);

    var networkDevices = await _unitOfWork.NetworkDevices.FirstAsync(requestExpression,
                                                                     cancellationToken)
      ?? throw new EntityNotFoundException(new ExpressionToStringConverter<NetworkDevice>().Convert(requestExpression));

    // Получаем уникальные идентификаторы портов из AggregatedPorts
    HashSet<Guid> aggregatedPortsIds = networkDevices.PortsOfNetworkDevice
        .Where(port => port.AggregatedPorts.Count != 0)
        .SelectMany(port => port.AggregatedPorts)
        .Select(item => item.Id)
        .ToHashSet();

    // Фильтруем PortsOfNetworkDevice, исключая повторяющиеся порты
    networkDevices.PortsOfNetworkDevice = [.. networkDevices.PortsOfNetworkDevice
          .Where(port => !aggregatedPortsIds.Contains(port.Id))
          .OrderBy(port => port.InterfaceName)];

    // Сортируем AggregatedPorts по InterfaceName
    foreach (var port in networkDevices.PortsOfNetworkDevice)
    {
      port.AggregatedPorts = [.. port.AggregatedPorts.OrderBy(aggregatedPort => aggregatedPort.InterfaceName)];
    }

    var networkDeviceDTO = _mapper.Map<NetworkDeviceDTO>(networkDevices);

    return networkDeviceDTO;
  }
}
