namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetById;

public record GetNetworkDeviceByIdQuery(Guid Id) : IQuery<NetworkDeviceDTO>;

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
    // Проверка существования сетевое устройство с данным ID
    var existingNetworkDevice = await _unitOfWork.NetworkDevices
                                                 .AnyByIdAsync(request.Id,
                                                               cancellationToken);

    // Если сетевое устройство не найдено — исключение
    if (!existingNetworkDevice)
      throw new EntityNotFoundById(typeof(NetworkDevice),
                                   request.Id.ToString());

    var networkDevice = await _unitOfWork.NetworkDevices.GetFirstWithChildrensByIdAsync(request.Id,
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

    var networkDeviceDTO = _mapper.Map<NetworkDeviceDTO>(networkDevice);

    return networkDeviceDTO;
  }
}
