using Microsoft.Extensions.Configuration;

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.Create;

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

    // Получаем назначения и бизнес-правила
    var assignments = await _unitOfWork.Assignments
                                       .GetAllAsync(cancellationToken);
    var businessRules = await _unitOfWork.BusinessRules
                                         .GetAllWithChildrenAsync(cancellationToken);

    // Проверяем наличие бизнес-правил
    if (!businessRules.Any())
      throw new InvalidOperationException($"Busness Rules collection is empty.");

    // Находим корневое бизнес-правило
    var rootRule = businessRules.FirstOrDefault(x => x.IsRoot == true)
      ?? throw new InvalidOperationException($"Busness Rules collection doesn't have root element.");

    var maxRepetitions = _configuration.GetValue<int>("Repetitions:Default");

    if (request.TypeOfNetworkDevice == TypeOfNetworkDevice.Juniper.ToString())
      maxRepetitions = _configuration.GetValue<int>("Repetitions:Juniper");

    if (request.TypeOfNetworkDevice == TypeOfNetworkDevice.Huawei.ToString())
      maxRepetitions = _configuration.GetValue<int>("Repetitions:Huawei");

    if (request.TypeOfNetworkDevice == TypeOfNetworkDevice.Extreme.ToString())
      maxRepetitions = _configuration.GetValue<int>("Repetitions:Extreme");

    // Обрабатываем дерево бизнес-правил
    await ProcessBusinessRuleTree(rootRule,
                                  businessRules,
                                  assignments,
                                  networkDevice,
                                  request,
                                  maxRepetitions,
                                  cancellationToken);

    // Заполняем ARP таблицу интерфейсов
    await FillARPTableForPorts(networkDevice,
                               request.Host,
                               request.Community,
                               maxRepetitions,
                               cancellationToken);

    await FillNetworkTableOfPort(networkDevice,
                                 request.Host,
                                 request.Community,
                                 maxRepetitions,
                                 cancellationToken);

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

    // Вставляем новое сетевое устройство в репозиторий
    _unitOfWork.NetworkDevices
                     .InsertOne(networkDevice);

    await _unitOfWork.CompleteAsync(cancellationToken);
    return Unit.Value;
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
                                           port.InterfaceName.StartsWith("NULL0") ||
                                           port.InterfaceName.StartsWith("Vlan"));
  }

  static void CleanExtreme(List<Port> portsOfNetworkDevice)
  {
    portsOfNetworkDevice.RemoveAll(port => port.InterfaceName.StartsWith("Management") ||
                                           port.InterfaceName.StartsWith("rtif") ||
                                           port.InterfaceName.StartsWith("Virtual") ||
                                           port.InterfaceName.StartsWith("VLAN"));
  }

  async Task ProcessBusinessRuleTree(BusinessRule rule,
                                     IEnumerable<BusinessRule> allRules,
                                     IEnumerable<Assignment> assignments,
                                     NetworkDevice networkDevice,
                                     CreateNetworkDeviceCommand request,
                                     int maxRepetitions,
                                     CancellationToken cancellationToken)
  {
    // Проверяем условие бизнес-правила
    bool resultOfEvaluateCondition = rule.Vendor == null || rule.Vendor == networkDevice.TypeOfNetworkDevice;

    // Если у бизнес-правила есть задание
    if (resultOfEvaluateCondition && rule.Assignment != null)
    {
      var assignment = assignments.Single(x => x.Id == rule.AssignmentId);

      // Выполняем задание в зависимости от типа запроса
      switch (assignment.TypeOfRequest)
      {
        case TypeOfRequest.get:
          {

            var singleValueToSet = (await _snmpCommandExecutor.GetCommand(request.Host,
                                                                          request.Community,
                                                                          assignment.OID,
                                                                          cancellationToken)).Data;
            HandleAssignment(networkDevice,
                             assignment,
                             singleValueToSet);
            break;
          }
        case TypeOfRequest.walk:
          {
            var multiplyValuesToSet = (await _snmpCommandExecutor.WalkCommand(request.Host,
                                                                              request.Community,
                                                                              assignment.OID,
                                                                              cancellationToken,
                                                                              repetitions: maxRepetitions))
                                                                 .Select(x => x.Data)
                                                                 .ToList();
            HandleAssignment(networkDevice,
                             assignment,
                             multiplyValuesToSet);
            break;
          }
      }
    }

    // Рекурсивно обрабатываем дочерние правила
    foreach (var child in rule.Children)
    {
      var childRule = allRules.FirstOrDefault(x => x.Id == child.Id);

      if (childRule != null)
        await ProcessBusinessRuleTree(childRule,
                                      allRules,
                                      assignments,
                                      networkDevice,
                                      request,
                                      maxRepetitions,
                                      cancellationToken);
    }
  }

  // Обработка задания с одним значением
  static void HandleAssignment(NetworkDevice networkDevice,
                               Assignment assignment,
                               string valueToSet)
  {
    // Проверка корректности TargetFieldName
    if (assignment == null || string.IsNullOrWhiteSpace(assignment.TargetFieldName))
      throw new ArgumentException("Assignment or TargetFieldName cannot be null or empty.");

    // Получение типа сетевого устройства
    var deviceType = typeof(NetworkDevice);

    // Получение свойства по имени из TargetFieldName
    var property = deviceType.GetProperty(assignment.TargetFieldName, BindingFlags.Public | BindingFlags.Instance)
      ?? throw new InvalidOperationException($"Property '{assignment.TargetFieldName}' not found on {deviceType.Name}.");

    // Конвертация значения и установка свойства
    var convertedValue = Convert.ChangeType(valueToSet, property.PropertyType);

    property.SetValue(obj: networkDevice,
                      value: convertedValue);
  }

  // Обработка задания со списком значений
  static void HandleAssignment(NetworkDevice networkDevice,
                               Assignment assignment,
                               List<string> valueToSet)
  {
    // Проверка корректности TargetFieldName
    if (assignment == null || string.IsNullOrWhiteSpace(assignment.TargetFieldName))
      throw new ArgumentException("Assignment or TargetFieldName cannot be null or empty.");

    // Разделение имени свойства на часть для NetworkDevice и Port
    string[] parts = assignment.TargetFieldName.Split('.');

    if (parts.Length != 2)
      throw new ArgumentException("TargetFieldName must be in the format 'Property.Field'.");

    string networkDeviceFieldName = parts[0]; // поле networkDevice
    string portFieldName = parts[1]; // поле Port

    // Получение типа NetworkDevice
    Type networkDeviceType = typeof(NetworkDevice);

    // Проверка существования свойства в NetworkDevice
    PropertyInfo portsProperty = networkDeviceType.GetProperty(networkDeviceFieldName, BindingFlags.Public | BindingFlags.Instance)!
      ?? throw new ArgumentException($"Property '{networkDeviceFieldName}' not found on {networkDeviceType.Name}.");

    // Проверка, является ли свойство коллекцией
    if (!typeof(IEnumerable).IsAssignableFrom(portsProperty.PropertyType) || portsProperty.PropertyType == typeof(string))
      throw new ArgumentException($"Property '{networkDeviceFieldName}' is not a collection.");

    // Получение типа элементов коллекции
    Type collectionType = portsProperty.PropertyType
                                       .GetGenericArguments()
                                       .FirstOrDefault()
      ?? throw new ArgumentException($"Cannot determine collection item type of {portFieldName}.");

    // Проверка существования свойства в типе элементов коллекции
    PropertyInfo portProperty = collectionType.GetProperty(portFieldName)!
      ?? throw new ArgumentException($"Property '{portFieldName}' not found on {collectionType.Name}.");

    // Проверка является ли свойства в типе элементов коллекции перечислением
    bool isEnumPortProperty = portProperty.PropertyType
                                          .IsEnum;

    // Получение текущей коллекции
    var collection = (ICollection)portsProperty.GetValue(networkDevice)!
      ?? throw new ArgumentException("Collection is null.");

    // Получение списка элементов коллекции
    var items = collection.Cast<object>()
                          .ToList();

    if (collection.Count == 0)
    {
      // Если коллекция пуста, создаем новые элементы и добавляем их в коллекцию
      for (int i = 0; i < valueToSet.Count; i++)
      {
        // Создание нового элемента коллекции
        var item = Activator.CreateInstance(collectionType);

        if (item != null)
        {
          if (isEnumPortProperty)
          {
            // Преобразование строки в значение enum
            var enumValue = Enum.Parse(portProperty.PropertyType, valueToSet[i]);
            portProperty.SetValue(item, enumValue);
          }
          else
          {
            // Конвертация строки в значение нужного типа
            var convertedValue = Convert.ChangeType(valueToSet[i], portProperty.PropertyType);
            portProperty.SetValue(item, convertedValue);

          }
          // Добавление элемента в коллекцию
          collection.GetType()
                    .GetMethod("Add")?
                    .Invoke(collection,
                            [item]);
        }
      }
    }
    else
    {
      // Если коллекция не пуста, обновляем существующие элементы
      for (int i = 0; i < items.Count; i++)
      {
        if (i >= valueToSet.Count)
          break;

        var item = items[i];

        if (isEnumPortProperty)
        {
          // Обновление значения свойства, если это перечисление
          var enumValue = Enum.Parse(portProperty.PropertyType, valueToSet[i]);
          portProperty.SetValue(item, enumValue);
        }
        else
        {
          // Конвертация и установка нового значения
          var convertedValue = Convert.ChangeType(valueToSet[i], portProperty.PropertyType);
          portProperty.SetValue(item, convertedValue);
        }
      }
    }
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
                                            iface,
                                            mac
                                          })
                                     .Zip(second: ipAddresses,
                                          resultSelector: (firstPair, ip) => new
                                          {
                                            Interface = firstPair.iface.Data,
                                            Mac = FormatMACAddress.Handle(firstPair.mac.Data),
                                            Ip = ip.Data
                                          });
    // Проверяем, что хотя бы один из запросов не вернул пустые данные
    var arpDictionary = arpEntries.GroupBy(entry => int.Parse(entry.Interface))
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
                                 IPAddress = entry.Value
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
                                                iface,
                                                address
                                              })
                                         .Zip(netMasks,
                                              (firstPair, mask) => new
                                              {
                                                Interface = int.Parse(firstPair.iface.Data),
                                                Address = firstPair.address.Data,
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
                                              portNums: FormatEgressPorts.HandleHuaweiHexString(x.Data)))
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
                                                     EgressPorts = FormatEgressPorts.HandleHuaweiHexString(egressPorts.Data)
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
