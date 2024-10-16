namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetByExpression;

internal class GetNetworkDevicesByIPQueryHandler(IUnitOfWork unitOfWork,
                                                 IMapper mapper)
  : IQueryHandler<GetNetworkDevicesByIPQuery, IEnumerable<NetworkDeviceDTO>>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<IEnumerable<NetworkDeviceDTO>> IRequestHandler<GetNetworkDevicesByIPQuery, IEnumerable<NetworkDeviceDTO>>.Handle(
      GetNetworkDevicesByIPQuery request,
      CancellationToken cancellationToken)
  {
    // Проверка формата IP-адреса
    if (!IPAddress.TryParse(request.IpAddress, out var ipToCheck))
      throw new ArgumentException("Invalid IP address format.", nameof(request.IpAddress));

    var ipToCheckNum = BitConverter.ToUInt32(ipToCheck.GetAddressBytes()
                                                      .Reverse()
                                                      .ToArray(), 0);
    // Определяем фильтрационное выражение
    var filterExpression = GetFilterExpression(ipToCheckNum);

    // Получаем сетевые устройства
    var networkDevice = await _unitOfWork.NetworkDevices
                                         .FindOneWithChildrenAsync(filterExpression: filterExpression,
                                                                   cancellationToken: cancellationToken)
      ?? throw new EntityNotFoundException(filterExpression.Name!);

    FilterPorts(networkDevice, ipToCheckNum, out HashSet<int> tags);

    // Определяем фильтрационное выражение
    filterExpression = GetRelatedFilterExpression(currentDevice: networkDevice.Id,
                                                  tags: tags);

    // Получаем связанные сетевые устройства на основе тегов VLAN
    var relatedDevices = await _unitOfWork.NetworkDevices
                                          .FindManyWithChildrenAsync(filterExpression: filterExpression,
                                                                     cancellationToken: cancellationToken);

    FilterPorts(relatedDevices, tags);

    List<NetworkDevice> networkDevices = [networkDevice, .. relatedDevices];

    // Фильтруем порты для каждого устройства
    foreach (var device in networkDevices)
    {
      // Получаем уникальные идентификаторы портов из AggregatedPorts
      HashSet<Guid> aggregatedPortsIds = device.PortsOfNetworkDevice
                                               .Where(port => port.AggregatedPorts.Count != 0)
                                               .SelectMany(port => port.AggregatedPorts)
                                               .Select(item => item.Id)
                                               .ToHashSet();

      HashSet<Guid> portsWith101Vlan = device.PortsOfNetworkDevice
                                             .SelectMany(port => port.VLANs)
                                             .Where(vlan => vlan.VLANTag == 101)
                                             .Select(item => item.Id)
                                             .ToHashSet();

      // Фильтруем PortsOfNetworkDevice, исключая повторяющиеся порты
      device.PortsOfNetworkDevice = device.PortsOfNetworkDevice
          .Where(port => !aggregatedPortsIds.Contains(port.Id))
          //.Where(port => !port.VLANs.Any(vlan => portsWith101Vlan.Contains(vlan.Id)))
          .ToList();
    }

    // Преобразуем в DTO с использованием AutoMapper
    return _mapper.Map<IEnumerable<NetworkDeviceDTO>>(networkDevices);
  }

  private static Expression<Func<NetworkDevice, bool>> GetRelatedFilterExpression(Guid currentDevice,
                                                                                  HashSet<int> tags)
  {
    return device => device.Id != currentDevice &&
           device.PortsOfNetworkDevice
                 .Any(port => port.VLANs
                                  .Any(vlan => tags.Contains(vlan.VLANTag)));
  }

  private static Expression<Func<NetworkDevice, bool>> GetFilterExpression(uint ipToCheckNum)
  {
    return x => x.PortsOfNetworkDevice.Any(port => port.NetworkTableOfInterface
                                                       .Any(networkEntry => (ipToCheckNum & networkEntry.Netmask) ==
                                                                            (networkEntry.NetworkAddress & networkEntry.Netmask)));
  }

  private static void FilterPorts(NetworkDevice networkDevice, uint ipToCheckNum, out HashSet<int> tags)
  {
    var filteredPorts = new List<Port>();
    tags = [];

    // Фильтруем порты по IP
    var filterByIp = networkDevice.PortsOfNetworkDevice
                                  .Where(port => port.NetworkTableOfInterface
                                                     .Any(networkEntry => (ipToCheckNum & networkEntry.Netmask) ==
                                                                          (networkEntry.NetworkAddress & networkEntry.Netmask)))
                                  .OrderBy(x => x.InterfaceName)
                                  .ToList();

    filteredPorts.AddRange(filterByIp);

    foreach (var port in filterByIp)
    {
      if (TryExtractNumber(port.InterfaceName, out int tag))
      {
        // Фильтруем порты по VLAN Tag
        var filterByVlanTag = networkDevice.PortsOfNetworkDevice
            .Where(p => p.VLANs.Any(vlan => vlan.VLANTag == tag));
        filteredPorts.AddRange(filterByVlanTag);
        tags.Add(tag);
      }
    }

    networkDevice.PortsOfNetworkDevice = filteredPorts;
  }

  private static void FilterPorts(IEnumerable<NetworkDevice> networkDevices, HashSet<int> tags)
  {
    foreach (var device in networkDevices)
    {
      // Фильтруем порты по VLANTag
      var filterByVLANTag = device.PortsOfNetworkDevice
                                  .Where(port => port.VLANs
                                                     .Any(vlan => tags.Contains(vlan.VLANTag)))
                                  .OrderBy(x => x.InterfaceName)
                                  .ToList();

      device.PortsOfNetworkDevice = filterByVLANTag;
    }
  }

  public static bool TryExtractNumber(string input, out int result)
  {
    string numberString = new(input.Where(char.IsDigit).ToArray());
    return int.TryParse(numberString, out result);
  }
}
