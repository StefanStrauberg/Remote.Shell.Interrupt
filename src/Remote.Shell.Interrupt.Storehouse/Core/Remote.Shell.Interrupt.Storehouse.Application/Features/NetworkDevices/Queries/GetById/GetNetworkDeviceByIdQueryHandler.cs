namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetById;

public record GetNetworkDeviceByIdQuery(Guid Id) : IQuery<NetworkDeviceDTO>;

internal class GetNetworkDeviceByIdQueryHandler(INetDevUnitOfWork netDevUnitOfWork,
                                                IMapper mapper)
  : IQueryHandler<GetNetworkDeviceByIdQuery, NetworkDeviceDTO>
{
  public async Task<NetworkDeviceDTO> Handle(GetNetworkDeviceByIdQuery request, CancellationToken cancellationToken)
  {
    var requestParameters = new RequestParameters
    {
      Filters = $"Id=={request.Id}"
    };

    // Проверка существования сетевое устройство с данным ID
    var existingNetworkDevice = await netDevUnitOfWork.NetworkDevices
                                                      .AnyByQueryAsync(requestParameters,
                                                                       cancellationToken);

    // Если сетевое устройство не найдено — исключение
    if (!existingNetworkDevice)
      throw new EntityNotFoundById(typeof(NetworkDevice),
                                   request.Id.ToString());

    var networkDevice = await netDevUnitOfWork.NetworkDevices
                                              .GetOneWithChildrensAsync(requestParameters,
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
}
