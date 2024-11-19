namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.Create;

public record CreateNetworkDeviceCommand(string Host, string Community, string TypeOfNetworkDevice) : ICommand;

internal class CreateNetworkDeviceCommandHandler(ISNMPCommandExecutor snmpCommandExecutor,
                                                 IUnitOfWork unitOfWork,
                                                 IConfiguration configuration)
  : ICommandHandler<CreateNetworkDeviceCommand, Unit>
{
  readonly ISNMPCommandExecutor _snmpCommandExecutor = snmpCommandExecutor
    ?? throw new ArgumentNullException(nameof(snmpCommandExecutor));
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IConfiguration _configuration = configuration
    ?? throw new ArgumentNullException(nameof(configuration));

  async Task<Unit> IRequestHandler<CreateNetworkDeviceCommand, Unit>.Handle(CreateNetworkDeviceCommand request,
                                                                            CancellationToken cancellationToken)
  {
    // Создаем новое сетевое устройство
    var networkDevice = new NetworkDevice
    {
      TypeOfNetworkDevice = Enum.Parse<TypeOfNetworkDevice>(request.TypeOfNetworkDevice),
      Host = request.Host
    };

    int maxRepetitions;

    if (request.TypeOfNetworkDevice == TypeOfNetworkDevice.Juniper.ToString())
      maxRepetitions = _configuration.GetValue<int>("Repetitions:Juniper");
    else if (request.TypeOfNetworkDevice == TypeOfNetworkDevice.Huawei.ToString())
      maxRepetitions = _configuration.GetValue<int>("Repetitions:Huawei");
    else if (request.TypeOfNetworkDevice == TypeOfNetworkDevice.Extreme.ToString())
      maxRepetitions = _configuration.GetValue<int>("Repetitions:Extreme");
    else
      maxRepetitions = _configuration.GetValue<int>("Repetitions:Default");

    var huaweiNew = _configuration.GetValue<bool>($"HuaweiNew:{request.Host}");

    await FillNetworkDevicesName(networkDevice,
                                 request.Host,
                                 request.Community,
                                 maxRepetitions,
                                 cancellationToken);

    // Вставляем новое сетевое устройство в репозиторий
    _unitOfWork.NetworkDevices
               .InsertOne(networkDevice);

    await FillPortsOfNetworkDevice(networkDevice,
                                   request.Host,
                                   request.Community,
                                   maxRepetitions,
                                   cancellationToken);

    // Вставляем порты сетевого устройство в репозиторий
    _unitOfWork.Ports
               .InsertMany(networkDevice.PortsOfNetworkDevice);

    // Заполняем ARP таблицу интерфейсов
    await FillARPTableForPorts(networkDevice,
                               request.Host,
                               request.Community,
                               maxRepetitions,
                               cancellationToken);

    _unitOfWork.ARPEntities
               .InsertMany(networkDevice.PortsOfNetworkDevice
                                        .SelectMany(x => x.ARPTableOfInterface));

    await FillMACTableForPorts(networkDevice,
                               request.Host,
                               request.Community,
                               maxRepetitions,
                               cancellationToken);

    _unitOfWork.MACEntities
               .InsertMany(networkDevice.PortsOfNetworkDevice
                                        .SelectMany(x => x.MACTable));

    await FillNetworkTableOfPort(networkDevice,
                                 request.Host,
                                 request.Community,
                                 maxRepetitions,
                                 cancellationToken);

    _unitOfWork.TerminatedNetworkEntities
               .InsertMany(networkDevice.PortsOfNetworkDevice
                                        .SelectMany(x => x.NetworkTableOfInterface));

    if (networkDevice.TypeOfNetworkDevice == TypeOfNetworkDevice.Juniper)
    {
      await FillPortVLANSForJuniper(networkDevice,
                                    request.Host,
                                    request.Community,
                                    maxRepetitions,
                                    cancellationToken);

      CleanJuniper(networkDevice.PortsOfNetworkDevice);

      await LinkAgregationPortsForJuniper(networkDevice,
                                          request.Host,
                                          request.Community,
                                          maxRepetitions,
                                          cancellationToken);

      CleanJuniperDots(networkDevice.PortsOfNetworkDevice);
    }
    else if (networkDevice.TypeOfNetworkDevice == TypeOfNetworkDevice.Huawei)
    {
      await FillPortVLANSForHuawei(networkDevice,
                                   request.Host,
                                   request.Community,
                                   maxRepetitions,
                                   huaweiNew,
                                   cancellationToken);

      await LinkAgregationPortsForHuawei(networkDevice,
                                         request.Host,
                                         request.Community,
                                         maxRepetitions,
                                         cancellationToken);

      CleanHuawei(networkDevice.PortsOfNetworkDevice);
    }
    else if (networkDevice.TypeOfNetworkDevice == TypeOfNetworkDevice.Extreme)
    {
      await FillPortVLANSForExtreme(networkDevice,
                                    request.Host,
                                    request.Community,
                                    maxRepetitions,
                                    cancellationToken);

      CleanExtreme(networkDevice.PortsOfNetworkDevice);

      await LinkAgregationPortsForExtreme(networkDevice,
                                          request.Host,
                                          request.Community,
                                          maxRepetitions,
                                          cancellationToken);
    }

    _unitOfWork.Complete();
    return Unit.Value;
  }

  private async Task FillNetworkDevicesName(NetworkDevice networkDevice,
                                            string host,
                                            string community,
                                            int maxRepetitions,
                                            CancellationToken cancellationToken)
  {
    var networkDeviceName = await _snmpCommandExecutor.GetCommand(host: host,
                                                                  community: community,
                                                                  oid: "1.3.6.1.2.1.1.5.0",
                                                                  cancellationToken: cancellationToken);
    networkDevice.NetworkDeviceName = networkDeviceName.Data;
  }

  private async Task FillPortsOfNetworkDevice(NetworkDevice networkDevice,
                                            string host,
                                            string community,
                                            int maxRepetitions,
                                            CancellationToken cancellationToken)
  {
    var interfacesNumbers = await _snmpCommandExecutor.WalkCommand(host: host,
                                                                   community: community,
                                                                   oid: "1.3.6.1.2.1.2.2.1.1",
                                                                   cancellationToken: cancellationToken,
                                                                   repetitions: maxRepetitions);

    var interfacesNames = await _snmpCommandExecutor.WalkCommand(host: host,
                                                                 community: community,
                                                                 oid: "1.3.6.1.2.1.2.2.1.2",
                                                                 cancellationToken: cancellationToken,
                                                                 repetitions: maxRepetitions);

    var interfacesTypes = await _snmpCommandExecutor.WalkCommand(host: host,
                                                                 community: community,
                                                                 oid: "1.3.6.1.2.1.2.2.1.3",
                                                                 cancellationToken: cancellationToken,
                                                                 repetitions: maxRepetitions);

    var interfacesSpeed = await _snmpCommandExecutor.WalkCommand(host: host,
                                                                 community: community,
                                                                 oid: "1.3.6.1.2.1.2.2.1.5",
                                                                 cancellationToken: cancellationToken,
                                                                 repetitions: maxRepetitions);

    var interfaceMacAddresses = await _snmpCommandExecutor.WalkCommand(host: host,
                                                                       community: community,
                                                                       oid: "1.3.6.1.2.1.2.2.1.6",
                                                                       cancellationToken: cancellationToken,
                                                                       toHex: true,
                                                                       repetitions: maxRepetitions);

    var interfacesStatuses = await _snmpCommandExecutor.WalkCommand(host: host,
                                                                    community: community,
                                                                    oid: "1.3.6.1.2.1.2.2.1.8",
                                                                    cancellationToken: cancellationToken,
                                                                    repetitions: maxRepetitions);

    var interfaceDescriptions = await _snmpCommandExecutor.WalkCommand(host: host,
                                                                       community: community,
                                                                       oid: "1.3.6.1.2.1.31.1.1.1.18",
                                                                       cancellationToken: cancellationToken,
                                                                       repetitions: maxRepetitions);


    // Упаковка коллекций с использованием Zip
    var zippedCollection = interfacesNumbers.Zip(interfacesNames, (number, name) =>
                                                                  new
                                                                  {
                                                                    number = int.Parse(number.Data),
                                                                    name = name.Data
                                                                  })
                                            .Zip(interfacesTypes, (first, response) =>
                                                                  new
                                                                  {
                                                                    first.number,
                                                                    first.name,
                                                                    type = Enum.Parse<PortType>(response.Data)
                                                                  })
                                            .Zip(interfacesSpeed, (second, response) =>
                                                                  new
                                                                  {
                                                                    second.number,
                                                                    second.name,
                                                                    second.type,
                                                                    speed = long.Parse(response.Data),
                                                                  })
                                            .Zip(interfacesStatuses, (third, response) =>
                                                                     new
                                                                     {
                                                                       third.number,
                                                                       third.name,
                                                                       third.type,
                                                                       third.speed,
                                                                       status = Enum.Parse<PortStatus>(response.Data)
                                                                     })
                                            .Zip(interfaceMacAddresses, (fourth, response) =>
                                                               new
                                                               {
                                                                 fourth.number,
                                                                 fourth.name,
                                                                 fourth.type,
                                                                 fourth.speed,
                                                                 fourth.status,
                                                                 mac = response.Data.Replace(' ', ':')
                                                               })
                                            .Zip(interfaceDescriptions, (fifth, response) =>
                                                               new
                                                               {
                                                                 fifth.number,
                                                                 fifth.name,
                                                                 fifth.type,
                                                                 fifth.speed,
                                                                 fifth.status,
                                                                 fifth.mac,
                                                                 description = response.Data
                                                               })
                                            .ToList();

    if (zippedCollection.Count == 0)
      return;

    var ports = new List<Port>(zippedCollection.Count);

    for (var i = 0; i < zippedCollection.Count; i++)
    {
      ports.Add(new Port
      {
        NetworkDeviceId = networkDevice.Id,
        NetworkDevice = networkDevice,
        InterfaceNumber = zippedCollection[i].number,
        InterfaceName = zippedCollection[i].name,
        InterfaceSpeed = zippedCollection[i].speed,
        InterfaceStatus = zippedCollection[i].status,
        MACAddress = zippedCollection[i].mac,
        Description = zippedCollection[i].description
      });
    }
    networkDevice.PortsOfNetworkDevice = ports;
  }

  private async Task FillMACTableForPorts(NetworkDevice networkDevice,
                                          string host,
                                          string community,
                                          int maxRepetitions,
                                          CancellationToken cancellationToken)
  {
    var macToVirNumbers = await _snmpCommandExecutor.WalkCommand(host: host,
                                                                 community: community,
                                                                 oid: "1.3.6.1.2.1.17.4.3.1.2",
                                                                 cancellationToken,
                                                                 repetitions: maxRepetitions);
    var virNumbers = await _snmpCommandExecutor.WalkCommand(host: host,
                                                            community: community,
                                                            oid: "1.3.6.1.2.1.17.1.4.1.1",
                                                            cancellationToken,
                                                            repetitions: maxRepetitions);

    var virNumToPort = await _snmpCommandExecutor.WalkCommand(host: host,
                                                              community: community,
                                                              oid: "1.3.6.1.2.1.17.1.4.1.2",
                                                              cancellationToken,
                                                              repetitions: maxRepetitions);

    if (macToVirNumbers.Count == 0 || virNumbers.Count == 0 || virNumToPort.Count == 0)
      return;

    // Проверка, что количество элементов одинаковое
    if (virNumbers.Count != virNumToPort.Count)
      throw new InvalidOperationException($"SNMP responses count mismatch for ARP data: virNumbers({virNumbers.Count}), virNumToPort({virNumToPort.Count})");

    var portsDict = networkDevice.PortsOfNetworkDevice
                                 .ToDictionary(port => port.InterfaceNumber, port => port);

    var macTable = macToVirNumbers.Select(response => new
    {
      num = int.Parse(response.Data),
      mac = FormatMACAddress.HandleMACTable(response.OID)
    }).GroupBy(entry => entry.num)
      .ToDictionary(group => group.Key,
                    group => group.Select(x => x.mac));

    // Объединяем данные в словарь по InterfaceNumber
    var interfaceTable = virNumbers.Zip(second: virNumToPort,
                                        resultSelector: (virNum, intNum) => new
                                        {
                                          virNum = int.Parse(virNum.Data),
                                          intNum = int.Parse(intNum.Data)
                                        });

    foreach (var item in interfaceTable)
    {
      if (macTable.TryGetValue(item.virNum, out var macs))
      {
        if (portsDict.TryGetValue(item.intNum, out var port))
        {
          foreach (var mac in macs)
          {
            port.MACTable
                .Add(new MACEntity
                {
                  MACAddress = mac,
                  PortId = port.Id,
                  Port = port
                });
          }
        }
      }
    }
  }

  static void CleanJuniper(List<Port> portsOfNetworkDevice)
  {
    portsOfNetworkDevice.RemoveAll(port => !(port.InterfaceName.StartsWith("xe") ||
                                             port.InterfaceName.StartsWith("irb") ||
                                             port.InterfaceName.StartsWith("ae")));
  }

  static void CleanJuniperDots(List<Port> portsOfNetworkDevice)
  {
    var aePorts = portsOfNetworkDevice.Where(x => x.InterfaceName.StartsWith("ae")).ToList();
    var aeGroups = aePorts.GroupBy(x => x.InterfaceType);
    var xePorts = portsOfNetworkDevice.Where(x => x.InterfaceName.StartsWith("xe")).ToList();
    var xeGroups = xePorts.GroupBy(x => x.InterfaceType);

    // Находим все порты без точки и создаем группы
    var aePortsWithoutDots = aePorts.Where(port => !port.InterfaceName.Contains('.')).ToList();
    var xePortsWithoutDots = xePorts.Where(port => !port.InterfaceName.Contains('.')).ToList();

    foreach (var port in aePortsWithoutDots)
    {
      var baseName = port.InterfaceName;

      // Находим порты с точкой с основанием baseName
      var lookingGroupWithDot = portsOfNetworkDevice.Where(p => p.InterfaceName.StartsWith(baseName + '.'))
                                                    .ToList();
      foreach (var item in lookingGroupWithDot)
      {
        // удаляем порты с точкой с основанием baseName
        portsOfNetworkDevice.Remove(item);
      }
    }
    foreach (var port in xePortsWithoutDots)
    {
      var baseName = port.InterfaceName;

      // Находим порты с точкой с основанием baseName
      var lookingGroupWithDot = portsOfNetworkDevice.Where(p => p.InterfaceName.StartsWith(baseName + '.'))
                                                    .ToList();
      foreach (var item in lookingGroupWithDot)
      {
        // удаляем порты с точкой с основанием baseName
        portsOfNetworkDevice.Remove(item);
      }
    }
  }

  static void CleanHuawei(List<Port> portsOfNetworkDevice)
  {
    portsOfNetworkDevice.RemoveAll(port => port.InterfaceName.StartsWith("InLoop") ||
                                           port.InterfaceName.StartsWith("MEth") ||
                                           port.InterfaceName.StartsWith("NULL0"));
  }

  static void CleanExtreme(List<Port> portsOfNetworkDevice)
  {
    portsOfNetworkDevice.RemoveAll(port => port.InterfaceName.StartsWith("Management") ||
                                           port.InterfaceName.StartsWith("Virtual") ||
                                           port.InterfaceName.StartsWith("VLAN"));
  }

  async Task FillARPTableForPorts(NetworkDevice networkDevice,
                                  string host,
                                  string community,
                                  int maxRepetitions,
                                  CancellationToken cancellationToken)
  {
    // Выполняем SNMP-запросы
    var interfaceNumbers = await _snmpCommandExecutor.WalkCommand(host: host,
                                                                  community: community,
                                                                  oid: "1.3.6.1.2.1.4.22.1.1",
                                                                  cancellationToken: cancellationToken,
                                                                  repetitions: maxRepetitions);
    var macAddresses = await _snmpCommandExecutor.WalkCommand(host: host,
                                                              community: community,
                                                              oid: "1.3.6.1.2.1.4.22.1.2",
                                                              cancellationToken,
                                                              toHex: true,
                                                              repetitions: maxRepetitions);
    var ipAddresses = await _snmpCommandExecutor.WalkCommand(host: host,
                                                             community: community,
                                                             oid: "1.3.6.1.2.1.4.22.1.3",
                                                             cancellationToken,
                                                             repetitions: maxRepetitions);

    if (interfaceNumbers.Count == 0 || macAddresses.Count == 0 || ipAddresses.Count == 0)
      throw new InvalidOperationException("One from SNMP requests receive empty result.");

    // Проверка, что количество элементов одинаковое
    if (interfaceNumbers.Count != macAddresses.Count || macAddresses.Count != ipAddresses.Count)
      throw new InvalidOperationException($"SNMP responses count mismatch for ARP data: interfaces({interfaceNumbers.Count}), macs({macAddresses.Count}), ips({ipAddresses.Count})");

    // Объединяем данные в словарь по InterfaceNumber
    var arpEntries = interfaceNumbers.Zip(second: macAddresses,
                                          resultSelector: (iface, mac) => new
                                          {
                                            Interface = int.Parse(iface.Data),
                                            Mac = FormatMACAddress.Handle(mac.Data)
                                          })
                                     .Zip(second: ipAddresses,
                                          resultSelector: (firstPair, ip) => new
                                          {
                                            firstPair.Interface,
                                            firstPair.Mac,
                                            Ip = ip.Data
                                          });
    // Проверяем, что хотя бы один из запросов не вернул пустые данные
    var arpDictionary = arpEntries.GroupBy(entry => entry.Interface)
                                  .ToDictionary(keySelector: group => group.Key,
                                                elementSelector: group => group.Select(e => new KeyValuePair<string, string>(e.Mac,
                                                                                                                             e.Ip))
                                  .ToList());

    var portsDictionary = networkDevice.PortsOfNetworkDevice
                                       .ToDictionary(kvp => kvp.InterfaceNumber);

    foreach (var arpEntry in arpDictionary)
    {
      // Ищем порт с соответствующим InterfaceNumber
      if (portsDictionary.TryGetValue(arpEntry.Key, out var port))
      {
        // Создаем новую таблицу, которая поддерживает несколько IP для одного MAC
        var arpTable = arpEntry.Value
                               .Select(entry => new ARPEntity
                               {
                                 MAC = entry.Key,
                                 IPAddress = entry.Value,
                                 PortId = port.Id,
                                 Port = port
                               })
                               .ToList();

        // Присваиваем заполненную ARP таблицу порту
        port.ARPTableOfInterface = arpTable;
      }
    }
  }

  async Task FillNetworkTableOfPort(NetworkDevice networkDevice,
                                    string host,
                                    string community,
                                    int maxRepetitions,
                                    CancellationToken cancellationToken)
  {
    // Выполняем SNMP-запрос для получения номеров интерфейсов
    var interfaceNumbers = await _snmpCommandExecutor.WalkCommand(host: host,
                                                                  community: community,
                                                                  oid: "1.3.6.1.2.1.4.20.1.2",
                                                                  cancellationToken: cancellationToken,
                                                                  repetitions: maxRepetitions);

    // Выполняем SNMP-запрос для получения IP-адресов
    var ipAddresses = await _snmpCommandExecutor.WalkCommand(host: host,
                                                             community: community,
                                                             oid: "1.3.6.1.2.1.4.20.1.1",
                                                             cancellationToken,
                                                             repetitions: maxRepetitions);

    // Выполняем SNMP-запрос для получения сетевых масок
    var netMasks = await _snmpCommandExecutor.WalkCommand(host: host,
                                                          community: community,
                                                          oid: "1.3.6.1.2.1.4.20.1.3",
                                                          cancellationToken,
                                                          repetitions: maxRepetitions);

    // Проверяем, что хотя бы один из запросов не вернул пустые данные
    if (interfaceNumbers.Count == 0 || ipAddresses.Count == 0 || ipAddresses.Count == 0)
      throw new InvalidOperationException("One from SNMP requests receive empty result.");

    // Проверяем, что количество элементов в каждом запросе совпадает
    if (interfaceNumbers.Count != ipAddresses.Count || ipAddresses.Count != ipAddresses.Count)
      throw new InvalidOperationException($"SNMP responses count mismatch for ARP data: interfaces({interfaceNumbers.Count}), macs({ipAddresses.Count}), ips({ipAddresses.Count})");

    // Объединяем результаты SNMP-запросов: interfaceNumber -> IP -> Mask
    var ipTableEntries = interfaceNumbers.Zip(ipAddresses,
                                              (iface, address) => new
                                              {
                                                Interface = int.Parse(iface.Data),
                                                Address = address.Data
                                              })
                                         .Zip(netMasks,
                                              (firstPair, mask) => new
                                              {
                                                firstPair.Interface,
                                                firstPair.Address,
                                                Mask = mask.Data
                                              });

    // Группируем данные по номерам интерфейсов и создаем словарь
    var arpDictionary = ipTableEntries.GroupBy(entry => entry.Interface)
                                      .ToDictionary(group => group.Key,
                                                    group => group.Select(e => new KeyValuePair<string, string>(e.Address,
                                                                                                                e.Mask))
                                      .ToList());

    var portsDictionary = networkDevice.PortsOfNetworkDevice
                                       .ToDictionary(p => p.InterfaceNumber);

    // Заполняем сетевую таблицу для каждого порта устройства, используя arpDictionary
    foreach (var arpEntry in arpDictionary)
    {
      // Ищем порт с соответствующим InterfaceNumber
      if (portsDictionary.TryGetValue(arpEntry.Key, out var port))
      {
        var networkTable = new List<TerminatedNetworkEntity>();

        // Добавляем записи IP и масок в сетевую таблицу порта
        foreach (var entry in arpEntry.Value)
        {
          var terminatedNetwork = new TerminatedNetworkEntity();
          terminatedNetwork.SetAddressAndMask(entry.Key, entry.Value);
          terminatedNetwork.PortId = port.Id;
          terminatedNetwork.Port = port;
          networkTable.Add(terminatedNetwork);
        }

        // Присваиваем заполненную сетевую таблицу порту
        port.NetworkTableOfInterface = networkTable;
      }
    }
  }

  async Task FillPortVLANSForJuniper(NetworkDevice networkDevice,
                                     string host,
                                     string community,
                                     int maxRepetitions,
                                     CancellationToken cancellationToken)
  {
    List<SNMPResponse> dot1dBasePort = [];
    List<SNMPResponse> dot1dBasePortIfIndex = [];
    List<SNMPResponse> dot1qVlanStaticName = [];
    List<SNMPResponse> dot1qVlanStaticEgressPorts = [];

    // Выполняем SNMP-запрос для получения Base Port
    dot1dBasePort = await _snmpCommandExecutor.WalkCommand(host: host,
                                                           community: community,
                                                           oid: "1.3.6.1.2.1.17.1.4.1.1",
                                                           cancellationToken: cancellationToken,
                                                           repetitions: maxRepetitions);

    // Выполняем SNMP-запрос для получения Port If Index
    dot1dBasePortIfIndex = await _snmpCommandExecutor.WalkCommand(host: host,
                                                                  community: community,
                                                                  oid: "1.3.6.1.2.1.17.1.4.1.2",
                                                                  cancellationToken: cancellationToken,
                                                                  repetitions: maxRepetitions);

    // Выполняем SNMP-запрос для получения VLAN Static Name
    dot1qVlanStaticName = await _snmpCommandExecutor.WalkCommand(host: host,
                                                                 community: community,
                                                                 oid: "1.3.6.1.2.1.17.7.1.4.3.1.1",
                                                                 cancellationToken,
                                                                 repetitions: maxRepetitions);

    // Выполняем SNMP-запрос для получения VLAN Egress Ports
    dot1qVlanStaticEgressPorts = await _snmpCommandExecutor.WalkCommand(host: host,
                                                                        community: community,
                                                                        oid: "1.3.6.1.2.1.17.7.1.4.3.1.2",
                                                                        cancellationToken,
                                                                        repetitions: maxRepetitions);

    // Проверяем, что хотя бы один из запросов не вернул пустые данные
    if (dot1dBasePort.Count == 0 || dot1qVlanStaticName.Count == 0 || dot1qVlanStaticEgressPorts.Count == 0 || dot1dBasePortIfIndex.Count == 0)
      throw new InvalidOperationException("One from SNMP requests receive empty result.");
    // Проверяем, что количество элементов в каждом запросе совпадает
    if (dot1dBasePort.Count != dot1dBasePortIfIndex.Count || dot1qVlanStaticName.Count != dot1qVlanStaticEgressPorts.Count)
      throw new InvalidOperationException($"SNMP responses count mismatch for data: dot1dBasePortIfIndex({dot1dBasePort.Count}) mismatch to dot1dBasePortIfIndex({dot1dBasePortIfIndex.Count}), dot1qVlanStaticName({dot1qVlanStaticName.Count}) mismatch to dot1qVlanStaticEgressPorts({dot1qVlanStaticEgressPorts.Count})");

    // Объединяем результаты SNMP-запросов
    var physicIfTable = dot1dBasePort.Zip(dot1dBasePortIfIndex, (basePort, ifIndex) => new
    {
      BasePort = int.Parse(basePort.Data),
      PortIfIndex = int.Parse(ifIndex.Data)
    }).ToDictionary(x => x.BasePort, x => x.PortIfIndex);

    var vlanTableEntries = dot1qVlanStaticName.Zip(dot1qVlanStaticEgressPorts, (vlanName, egressPorts) => new
    {
      VlanTag = OIDGetNumbers.HandleLast(vlanName.OID),
      VlanName = vlanName.Data,
      EgressPorts = FormatEgressPorts.HandleJuniperData(egressPorts.Data)
    }).ToList();

    // Инициализируем словарь для быстрого поиска портов по их InterfaceNumber
    var portsDictionary = networkDevice.PortsOfNetworkDevice
                                       .ToDictionary(port => port.InterfaceNumber);

    var vlansToAdd = new HashSet<VLAN>();

    // Проходим по результатам vlanTableEntries
    foreach (var vlanEntry in vlanTableEntries)
    {
      VLAN vlanToCreate = new()
      {
        VLANTag = vlanEntry.VlanTag,
        VLANName = RemoveTrailingPlusDigit.Handle(vlanEntry.VlanName)
      };
      // Проходим по каждому egress-порту для текущего VLAN
      foreach (var egressPort in vlanEntry.EgressPorts)
      {
        vlansToAdd.Add(vlanToCreate);
        // Находим соответствие BasePort с PortIfIndex
        if (physicIfTable.TryGetValue(egressPort, out int portIfIndex))
        {
          // Ищем порт в коллекции PortsOfNetworkDevice по InterfaceNumber
          if (portsDictionary.TryGetValue(portIfIndex, out var matchingPort))
          {
            // Если коллекция VLANs не инициализирована, инициализируем её
            matchingPort.VLANs ??= [];

            // Добавляем новый VLAN в порт
            matchingPort.VLANs.Add(vlanToCreate);
          }
        }
      }
    }

    _unitOfWork.VLANs
               .InsertMany(vlansToAdd);
  }

  async Task LinkAgregationPortsForJuniper(NetworkDevice networkDevice,
                                           string host,
                                           string community,
                                           int maxRepetitions,
                                           CancellationToken cancellationToken)
  {
    List<SNMPResponse> ifStackTable = [];

    // Выполняем SNMP-запрос для получения IF-MIB::ifStackTable
    ifStackTable = await _snmpCommandExecutor.WalkCommand(host: host,
                                                          community: community,
                                                          oid: "1.3.6.1.2.1.31.1.2.1.3",
                                                          cancellationToken,
                                                          repetitions: maxRepetitions);

    // Проверяем, что хотя бы один из запросов не вернул пустые данные
    if (ifStackTable.Count == 0)
      return;

    var aePorts = networkDevice.PortsOfNetworkDevice
                               .Where(x => x.InterfaceType == PortType.ieee8023adLag ||
                                           x.InterfaceType == PortType.propVirtual)
                               .ToList();

    var xePorts = networkDevice.PortsOfNetworkDevice
                               .Where(x => x.InterfaceType == PortType.ethernetCsmacd ||
                                           x.InterfaceType == PortType.propVirtual ||
                                           x.InterfaceType == PortType.l2vlan)
                               .ToList();

    var aeGroupedPorts = GroupPorts(aePorts);
    var xeGroupedPorts = GroupPorts(xePorts);

    // Создаем набор ключей для aePorts и xePorts
    var aeKeys = aeGroupedPorts.SelectMany(g => g.Key)
                               .ToHashSet();

    // Получаем ключи из ifStackTable
    var aggKeySet = ifStackTable.Select(x => (aeNum: OIDGetNumbers.HandleLastButOne(x.OID),
                                              portNum: OIDGetNumbers.HandleLast(x.OID)))
                                .Where(x => x.aeNum != 0 && x.portNum != 0)
                                .Where(x => aeKeys.Contains(x.aeNum))
                                .ToHashSet();

    foreach (var (aeNum, portNum) in aggKeySet)
    {
      // Получаем нужную группу
      var aggGroup = aeGroupedPorts.FirstOrDefault(kvp => kvp.Key
                                                             .Contains(aeNum));
      var exGroup = xeGroupedPorts.FirstOrDefault(kvp => kvp.Key
                                                            .Contains(portNum));

      // Проверяем, что группы найдены
      if (aggGroup.Value != null && exGroup.Value != null)
      {
        // Получаем первый порт из каждой группы
        var aggPort = aggGroup.Value
                              .First();
        var exPort = exGroup.Value
                            .First();

        if (!aggPort.AggregatedPorts
                    .Contains(exPort))
          aggPort.AggregatedPorts
                 .Add(exPort);
      }
      else
      {
        continue;
      }
    }
  }

  async Task LinkAgregationPortsForHuawei(NetworkDevice networkDevice,
                                          string host,
                                          string community,
                                          int maxRepetitions,
                                          CancellationToken cancellationToken)
  {
    List<SNMPResponse> ifStackTable = [];

    // Выполняем SNMP-запрос для получения IF-MIB::ifStackTable
    ifStackTable = await _snmpCommandExecutor.WalkCommand(host: host,
                                                          community: community,
                                                          oid: "1.3.6.1.2.1.31.1.2.1.3",
                                                          cancellationToken,
                                                          repetitions: maxRepetitions);


    var portsDictionary = networkDevice.PortsOfNetworkDevice
                                       .ToDictionary(p => p.InterfaceNumber);

    // Проверяем, что хотя бы один из запросов не вернул пустые данные
    if (ifStackTable.Count == 0)
    {
      // Выполняем SNMP-запрос для получения IF-MIB::ifStackTable
      ifStackTable = await _snmpCommandExecutor.WalkCommand(host: host,
                                                            community: community,
                                                            oid: "1.2.840.10006.300.43.1.1.2.1.1",
                                                            cancellationToken,
                                                            toHex: true,
                                                            repetitions: maxRepetitions);

      var aggKeys = ifStackTable.Select(x => (aeEnum: OIDGetNumbers.HandleLast(x.OID),
                                              portNums: FormatEgressPorts.HandleHuaweiHexStringOld(x.Data)))
                                .Where(x => x.portNums.Length != 0)
                                .ToList();

      foreach (var (aeNum, portNums) in aggKeys)
      {
        if (portsDictionary.TryGetValue(aeNum, out var aePort))
        {
          if (portNums.Length == 0)
            continue;

          foreach (var portNum in portNums)
          {
            var lookingForPort = portNum > 48 ? portNum + 12 : portNum + 6;

            if (portsDictionary.TryGetValue(lookingForPort, out var port))
              aePort.AggregatedPorts.Add(port);
          }
        }
      }
      return;
    }

    // Получаем ключи из ifStackTable
    var aggKeySet = ifStackTable.Select(x => (aeNum: OIDGetNumbers.HandleLastButOne(x.OID),
                                              portNum: OIDGetNumbers.HandleLast(x.OID)))
                                .Where(x => x.aeNum != 0 && x.portNum != 0)
                                .ToHashSet();


    foreach (var (aeNum, portNum) in aggKeySet)
    {
      // Проверяем, что группы найдены
      if (portsDictionary.TryGetValue(aeNum, out var aePort) && portsDictionary.TryGetValue(portNum, out var gePort))
      {
        if (!aePort.AggregatedPorts.Any(x => x.InterfaceNumber == gePort.InterfaceNumber))
          aePort.AggregatedPorts.Add(gePort);
      }
      else
      {
        continue;
      }
    }
  }

  async Task LinkAgregationPortsForExtreme(NetworkDevice networkDevice,
                                           string host,
                                           string community,
                                           int maxRepetitions,
                                           CancellationToken cancellationToken)
  {
    List<SNMPResponse> ifStackTable = [];

    // Выполняем SNMP-запрос для получения IF-MIB::ifStackTable
    ifStackTable = await _snmpCommandExecutor.WalkCommand(host: host,
                                                          community: community,
                                                          oid: "1.3.6.1.4.1.1916.1.4.3.1.4",
                                                          cancellationToken,
                                                          repetitions: maxRepetitions);

    // Проверяем, что хотя бы один из запросов не вернул пустые данные
    if (ifStackTable.Count == 0)
      return;

    var stackPorts = networkDevice.PortsOfNetworkDevice
                                  .ToDictionary(port => port.InterfaceNumber, port => port);

    // Получаем ключи из ifStackTable
    var aggKeySet = ifStackTable.Select(x => (aeNum: OIDGetNumbers.HandleLastButOne(x.OID),
                                              portNum: OIDGetNumbers.HandleLast(x.OID)))
                                .Where(x => x.aeNum != 0 && x.portNum != 0)
                                .Where(x => x.aeNum != x.portNum)
                                .ToHashSet();

    foreach (var (aeNum, portNum) in aggKeySet)
    {
      // Проверяем, что группы найдены
      if (stackPorts.TryGetValue(aeNum, out var aePort) && stackPorts.TryGetValue(portNum, out var gePort))
      {
        if (!aePort.AggregatedPorts.Any(x => x.InterfaceNumber == gePort.InterfaceNumber))
          aePort.AggregatedPorts.Add(gePort);
      }
      else
      {
        continue;
      }
    }
  }

  static Dictionary<HashSet<int>, List<Port>> GroupPorts(List<Port> ports)
  {
    // Создаем словарь для группировки
    var baseGroups = new Dictionary<HashSet<int>, List<Port>>();
    var lookingFor = new StringBuilder();

    // Находим все порты без точки и создаем группы
    var portsWithoutDots = ports.Where(port => !port.InterfaceName
                                                    .Contains('.'))
                                .ToList();

    var group = new List<Port>();
    var keys = new HashSet<int>(); // Начальный ключ

    foreach (var port in portsWithoutDots)
    {
      lookingFor.Append(port.InterfaceName);
      lookingFor.Append('.');

      keys.Add(port.InterfaceNumber);
      group.Add(port);

      // Находим порты с точкой и добавляем их в группу
      var lookingGroupWithDot = ports.Where(p => p.InterfaceName
                                                  .StartsWith(lookingFor.ToString()))
                                     .ToList();

      group.AddRange(lookingGroupWithDot);
      keys.UnionWith(lookingGroupWithDot.Select(p => p.InterfaceNumber)); // Добавляем ключи

      // Добавляем группу в словарь
      baseGroups[keys] = group;

      lookingFor.Clear();
      group.Clear();
      keys.Clear();
    }

    return baseGroups;
  }

  async Task FillPortVLANSForHuawei(NetworkDevice networkDevice,
                                    string host,
                                    string community,
                                    int maxRepetitions,
                                    bool huaweiNew,
                                    CancellationToken cancellationToken)
  {
    List<SNMPResponse> dot1dBasePort = [];
    List<SNMPResponse> dot1dBasePortIfIndex = [];
    List<SNMPResponse> dot1qVlanStaticName = [];
    List<SNMPResponse> dot1qVlanStaticEgressPorts = [];

    // Выполняем SNMP-запрос для получения Base Port
    dot1dBasePort = await _snmpCommandExecutor.WalkCommand(host: host,
                                                           community: community,
                                                           oid: "1.3.6.1.2.1.17.1.4.1.1",
                                                           cancellationToken: cancellationToken,
                                                           repetitions: maxRepetitions);

    // Выполняем SNMP-запрос для получения Port If Index
    dot1dBasePortIfIndex = await _snmpCommandExecutor.WalkCommand(host: host,
                                                                  community: community,
                                                                  oid: "1.3.6.1.2.1.17.1.4.1.2",
                                                                  cancellationToken: cancellationToken,
                                                                  repetitions: maxRepetitions);

    // Выполняем SNMP-запрос для получения VLAN Static Name
    dot1qVlanStaticName = await _snmpCommandExecutor.WalkCommand(host: host,
                                                                 community: community,
                                                                 oid: "1.3.6.1.2.1.17.7.1.4.3.1.1",
                                                                 cancellationToken,
                                                                 repetitions: maxRepetitions);

    // Выполняем SNMP-запрос для получения VLAN Egress Ports
    dot1qVlanStaticEgressPorts = await _snmpCommandExecutor.WalkCommand(host: host,
                                                                        community: community,
                                                                        oid: "1.3.6.1.2.1.17.7.1.4.2.1.4.0",
                                                                        cancellationToken,
                                                                        toHex: true,
                                                                        repetitions: maxRepetitions);

    // Проверяем, что хотя бы один из запросов не вернул пустые данные
    if (dot1dBasePort.Count == 0 || dot1qVlanStaticName.Count == 0 || dot1qVlanStaticEgressPorts.Count == 0 || dot1dBasePortIfIndex.Count == 0)
      throw new InvalidOperationException("One from SNMP requests receive empty result.");
    // Проверяем, что количество элементов в каждом запросе совпадает
    if (dot1dBasePort.Count != dot1dBasePortIfIndex.Count || dot1qVlanStaticName.Count != dot1qVlanStaticEgressPorts.Count)
      throw new InvalidOperationException($"SNMP responses count mismatch for ARP data: dot1dBasePortIfIndex({dot1dBasePort.Count}) mismatch to dot1dBasePortIfIndex({dot1dBasePortIfIndex.Count}), dot1qVlanStaticName({dot1qVlanStaticName.Count}) mismatch to dot1qVlanStaticEgressPorts({dot1qVlanStaticEgressPorts.Count})");

    // Объединяем результаты SNMP-запросов
    var physicIfTable = dot1dBasePort.Zip(dot1dBasePortIfIndex,
                                          (basePort, ifIndex) => new
                                          {
                                            BasePort = int.Parse(basePort.Data),
                                            PortIfIndex = int.Parse(ifIndex.Data)
                                          })
                                     .ToDictionary(x => x.BasePort, x => x.PortIfIndex);

    var vlanTableEntries = dot1qVlanStaticName.Zip(dot1qVlanStaticEgressPorts,
                                                   (vlanName, egressPorts) => new
                                                   {
                                                     VlanTag = OIDGetNumbers.HandleLast(vlanName.OID),
                                                     VlanName = vlanName.Data,
                                                     EgressPorts = huaweiNew == true ? FormatEgressPorts.HandleHuaweiHexStringNew(egressPorts.Data) : FormatEgressPorts.HandleHuaweiHexStringOld(egressPorts.Data)
                                                     //EgressPorts = FormatEgressPorts.HandleHuaweiHexString(egressPorts.Data)
                                                   })
                                              .ToList();

    // Инициализируем словарь для быстрого поиска портов по их InterfaceNumber
    var portsDictionary = networkDevice.PortsOfNetworkDevice
        .ToDictionary(port => port.InterfaceNumber);

    var vlansToAdd = new HashSet<VLAN>();

    // Проходим по результатам vlanTableEntries
    foreach (var vlanEntry in vlanTableEntries)
    {
      VLAN vlanToCreate = new()
      {
        VLANTag = vlanEntry.VlanTag,
        VLANName = vlanEntry.VlanName
      };
      // Проходим по каждому egress-порту для текущего VLAN
      foreach (var egressPort in vlanEntry.EgressPorts)
      {
        vlansToAdd.Add(vlanToCreate);
        // Находим соответствие BasePort с PortIfIndex
        if (physicIfTable.TryGetValue(egressPort, out int portIfIndex))
        {
          // Ищем порт в коллекции PortsOfNetworkDevice по InterfaceNumber
          if (portsDictionary.TryGetValue(portIfIndex, out var matchingPort))
          {
            // Если коллекция VLANs не инициализирована, инициализируем её
            matchingPort.VLANs ??= [];

            // Добавляем новый VLAN в порт
            matchingPort.VLANs.Add(vlanToCreate);
          }
        }
      }
    }

    _unitOfWork.VLANs
               .InsertMany(vlansToAdd);
  }

  async Task FillPortVLANSForExtreme(NetworkDevice networkDevice,
                                            string host,
                                            string community,
                                            int maxRepetitions,
                                            CancellationToken cancellationToken)
  {
    List<SNMPResponse> dot1qVlanStaticEgressPorts = [];
    List<SNMPResponse> dot1qVlanStaticName = [];
    List<SNMPResponse> dot1qVlanStaticNumber = [];
    List<SNMPResponse> dot1qVlanStaticTag = [];

    // Выполняем SNMP-запрос для получения VlanNumber
    dot1qVlanStaticNumber = await _snmpCommandExecutor.WalkCommand(host,
                                                                   community,
                                                                   "1.3.6.1.4.1.1916.1.2.1.2.1.1",
                                                                   cancellationToken,
                                                                   repetitions: maxRepetitions);

    // Выполняем SNMP-запрос для получения VlanName
    dot1qVlanStaticName = await _snmpCommandExecutor.WalkCommand(host,
                                                                 community,
                                                                 "1.3.6.1.4.1.1916.1.2.1.2.1.2",
                                                                 cancellationToken,
                                                                 repetitions: maxRepetitions);
    // Выполняем SNMP-запрос для получения VlanTag
    dot1qVlanStaticTag = await _snmpCommandExecutor.WalkCommand(host,
                                                                community,
                                                                "1.3.6.1.4.1.1916.1.2.1.2.1.10",
                                                                cancellationToken,
                                                                repetitions: maxRepetitions);

    // Выполняем SNMP-запрос для получения Base Port & VLAN Egress Ports
    dot1qVlanStaticEgressPorts = await _snmpCommandExecutor.WalkCommand(host: host,
                                                                        community: community,
                                                                        oid: "1.3.6.1.4.1.1916.1.4.17.1.2",
                                                                        cancellationToken,
                                                                        repetitions: maxRepetitions);

    // Проверяем, что хотя бы запрос не вернул пустые данные
    if (dot1qVlanStaticEgressPorts.Count == 0)
      return;

    // Объединяем результаты SNMP-запросов
    var vlanTable = dot1qVlanStaticNumber.Zip(dot1qVlanStaticName,
                                              (vlanNumber, vlanName) => new
                                              {
                                                vlanNumber,
                                                vlanName
                                              })
                                        .Zip(dot1qVlanStaticTag,
                                             (firstPair, vlanTag) => new
                                             {
                                               VlanNumber = int.Parse(firstPair.vlanNumber.Data),
                                               VlanName = firstPair.vlanName.Data,
                                               VlanTag = int.Parse(vlanTag.Data)
                                             })
                                        .ToDictionary(x => x.VlanNumber);

    var egressPorts = dot1qVlanStaticEgressPorts.Select(response =>
                                                        {
                                                          var oidParts = response.OID.Split('.');

                                                          if (!int.TryParse(oidParts[^1], out int vlanNumber))
                                                            throw new FormatException("Unable to parse port index from OID.");

                                                          if (!int.TryParse(oidParts[^2], out int portIfIndex))
                                                            throw new FormatException("Unable to parse port index from OID.");

                                                          return new
                                                          {
                                                            PortIfIndex = portIfIndex,
                                                            VlanNumber = vlanNumber
                                                          };
                                                        })
                                                .ToList();

    // Инициализируем словарь для быстрого поиска портов по их InterfaceNumber
    var portsDictionary = networkDevice.PortsOfNetworkDevice
        .ToDictionary(port => port.InterfaceNumber);

    var vlansToAdd = new HashSet<VLAN>();

    // Проходим по результатам vlanEntries
    foreach (var egressPort in egressPorts)
    {
      if (vlanTable.TryGetValue(egressPort.VlanNumber, out var vlan))
      {
        VLAN vlanToCreate = new()
        {
          VLANTag = vlan.VlanTag,
          VLANName = vlan.VlanName
        };
        // Ищем порт в коллекции PortsOfNetworkDevice по InterfaceNumber
        if (portsDictionary.TryGetValue(egressPort.PortIfIndex, out var matchingPort))
        {
          vlansToAdd.Add(vlanToCreate);
          // Если коллекция VLANs не инициализирована, инициализируем её
          matchingPort.VLANs ??= [];

          // Добавляем новый VLAN в порт
          matchingPort.VLANs.Add(vlanToCreate);
        }
      }
    }

    _unitOfWork.VLANs
               .InsertMany(vlansToAdd);
  }
}
