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
    var networkDevice = await _unitOfWork.NetworkDevices.GetFirstWithChildrensByIdAsync(request.Id,
                                                                                        cancellationToken)
      ?? throw new EntityNotFoundById(typeof(NetworkDevice),
                                      request.Id.ToString());

    // Получаем уникальные идентификаторы портов из AggregatedPorts
    HashSet<Guid> aggregatedPortsIds = networkDevice.PortsOfNetworkDevice
        .Where(port => port.AggregatedPorts.Count != 0)
        .SelectMany(port => port.AggregatedPorts)
        .Select(item => item.Id)
        .ToHashSet();

    // Фильтруем PortsOfNetworkDevice, исключая повторяющиеся порты
    networkDevice.PortsOfNetworkDevice = [.. networkDevice.PortsOfNetworkDevice
          .Where(port => !aggregatedPortsIds.Contains(port.Id))
          .OrderBy(port => port.InterfaceName)];

    var networkDeviceDTO = _mapper.Map<NetworkDeviceDTO>(networkDevice);

    return networkDeviceDTO;
  }
}
