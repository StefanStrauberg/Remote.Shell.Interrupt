namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetAll;

internal class GetNetworkDevicesQueryHandler(IUnitOfWork unitOfWork,
                                             IMapper mapper)
  : IQueryHandler<GetNetworkDevicesQuery, IEnumerable<NetworkDeviceDTO>>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<IEnumerable<NetworkDeviceDTO>> IRequestHandler<GetNetworkDevicesQuery, IEnumerable<NetworkDeviceDTO>>.Handle(GetNetworkDevicesQuery request,
                                                                                                                          CancellationToken cancellationToken)
  {
    var networkDevices = await _unitOfWork.NetworkDevices
                                          .GetAllAsync(cancellationToken);
    //.GetAllWithChildrenAsync(cancellationToken);

    // // Фильтруем порты для каждого устройства
    // foreach (var device in networkDevices)
    // {
    //   // Получаем уникальные идентификаторы портов из AggregatedPorts
    //   HashSet<Guid> aggregatedPortsIds = device.PortsOfNetworkDevice
    //       .Where(port => port.AggregatedPorts.Count != 0)
    //       .SelectMany(port => port.AggregatedPorts)
    //       .Select(item => item.Id)
    //       .ToHashSet();

    //   // Фильтруем PortsOfNetworkDevice, исключая повторяющиеся порты
    //   device.PortsOfNetworkDevice = [.. device.PortsOfNetworkDevice
    //       .Where(port => !aggregatedPortsIds.Contains(port.Id))
    //       .OrderBy(port => port.InterfaceName)];

    //   // Сортируем AggregatedPorts по InterfaceName
    //   foreach (var port in device.PortsOfNetworkDevice)
    //   {
    //     port.AggregatedPorts = [.. port.AggregatedPorts.OrderBy(aggregatedPort => aggregatedPort.InterfaceName)];
    //   }
    // }

    var networkDevicesDTOs = _mapper.Map<IEnumerable<NetworkDeviceDTO>>(networkDevices);

    return networkDevicesDTOs;
  }
}
