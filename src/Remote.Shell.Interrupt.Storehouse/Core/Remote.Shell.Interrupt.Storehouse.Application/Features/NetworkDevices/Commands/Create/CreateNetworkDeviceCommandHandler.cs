namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.Create;

internal class CreateNetworkDeviceCommandHandler(INetworkDeviceRepository networkDeviceRepository,
                                                 IBusinessRuleRepository businessRulesRepository,
                                                 IAssignmentRepository assignmentRepository,
                                                 ISNMPCommandExecutor snmpCommandExecutor)
  : ICommandHandler<CreateNetworkDeviceCommand, Unit>
{
  readonly INetworkDeviceRepository _networkDeviceRepository = networkDeviceRepository
    ?? throw new ArgumentNullException(nameof(networkDeviceRepository));
  readonly IBusinessRuleRepository _businessRulesRepository = businessRulesRepository
    ?? throw new ArgumentNullException(nameof(businessRulesRepository));
  readonly IAssignmentRepository _assignmentRepository = assignmentRepository
    ?? throw new ArgumentNullException(nameof(assignmentRepository));
  readonly ISNMPCommandExecutor _SNMPCommandExecutor = snmpCommandExecutor
    ?? throw new ArgumentNullException(nameof(snmpCommandExecutor));

  async Task<Unit> IRequestHandler<CreateNetworkDeviceCommand, Unit>.Handle(CreateNetworkDeviceCommand request,
                                                                            CancellationToken cancellationToken)
  {
    // // Проверяем существует ли уже сетевое устройство с таким Host
    // var filterNetworkDevice = (Expression<Func<NetworkDevice, bool>>)(x => x.Host == request.Host);
    // var existingNetworkDevice = await _networkDeviceRepository.ExistsAsync(filterExpression: filterNetworkDevice,
    //                                                                        cancellationToken: cancellationToken);

    // // Если устройство уже существует, выбрасываем исключение
    // if (existingNetworkDevice)
    //   throw new EntityAlreadyExists(request.Host);

    // Создаем новое сетевое устройство
    var networkDevice = new NetworkDevice
    {
      NetworkDeviceName = request.NetworkDeviceName,
      Host = request.Host,
      Created = DateTime.UtcNow,
      Modified = DateTime.UtcNow
    };

    // Получаем назначения и бизнес-правила
    var assignments = await _assignmentRepository.GetAllAsync(cancellationToken);
    var businessRules = await _businessRulesRepository.GetAllAsync(cancellationToken);

    // Проверяем наличие бизнес-правил
    if (!businessRules.Any())
      throw new InvalidOperationException($"Busness Rules collection is empty.");

    // Находим корневое бизнес-правило
    var rootRule = businessRules.FirstOrDefault(x => x.IsRoot == true)
      ?? throw new InvalidOperationException($"Busness Rules collection doesn't have root element.");

    // Обрабатываем дерево бизнес-правил
    await ProcessBusinessRuleTree(rule: rootRule,
                                  allRules: businessRules,
                                  assignments: assignments,
                                  networkDevice: networkDevice,
                                  request: request,
                                  cancellationToken: cancellationToken);

    // Заполняем ARP таблицу интерфейсов
    await FillARPTableForPorts(networkDevice: networkDevice,
                               host: request.Host,
                               community: request.Community,
                               cancellationToken: cancellationToken);

    await FillNetworkTableOfPort(networkDevice: networkDevice,
                                 host: request.Host,
                                 community: request.Community,
                                 cancellationToken: cancellationToken);

    // Вставляем новое сетевое устройство в репозиторий
    await _networkDeviceRepository.InsertOneAsync(document: networkDevice,
                                                  cancellationToken: cancellationToken);
    return Unit.Value;
  }
  async Task ProcessBusinessRuleTree(BusinessRule rule,
                                     IEnumerable<BusinessRule> allRules,
                                     IEnumerable<Assignment> assignments,
                                     NetworkDevice networkDevice,
                                     CreateNetworkDeviceCommand request,
                                     CancellationToken cancellationToken)
  {
    // Проверяем условие бизнес-правила
    bool resultOfEvaluateCondition = rule.Condition == null || await rule.EvaluateConditionAsync(networkDevice);

    // Если у бизнес-правила есть задание
    if (resultOfEvaluateCondition)
    {
      if (rule.AssignmentId.HasValue)
      {
        var assignment = assignments.Single(x => x.Id == rule.AssignmentId.Value);

        // Выполняем задание в зависимости от типа запроса
        switch (assignment.TypeOfRequest)
        {
          case TypeOfRequest.get:
            var singleValueToSet = (await _SNMPCommandExecutor.GetCommand(request.Host,
                                                                          request.Community,
                                                                          assignment.OID,
                                                                          cancellationToken)).Data;
            HandleAssignment(networkDevice,
                             assignment,
                             singleValueToSet);
            break;
          case TypeOfRequest.walk:
            var multiplyValuesToSet = (await _SNMPCommandExecutor.WalkCommand(request.Host,
                                                                              request.Community,
                                                                              assignment.OID,
                                                                              cancellationToken))
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
    foreach (var childId in rule.Children)
    {
      var childRule = allRules.FirstOrDefault(x => x.Id == childId);

      if (childRule != null)
        await ProcessBusinessRuleTree(childRule,
                                      allRules,
                                      assignments,
                                      networkDevice,
                                      request,
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
    var property = deviceType.GetProperty(name: assignment.TargetFieldName,
                                          bindingAttr: BindingFlags.Public | BindingFlags.Instance)
      ?? throw new InvalidOperationException($"Property '{assignment.TargetFieldName}' not found on {deviceType.Name}.");

    // Конвертация значения и установка свойства
    var convertedValue = Convert.ChangeType(value: valueToSet,
                                            conversionType: property.PropertyType);
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
    PropertyInfo portsProperty = networkDeviceType.GetProperty(name: networkDeviceFieldName,
                                                               bindingAttr: BindingFlags.Public | BindingFlags.Instance)!
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
          // Установка идентификатора и временных меток
          var idProperty = collectionType.GetProperty(nameof(BaseEntity.Id));
          var createdProperty = collectionType.GetProperty(nameof(BaseEntity.Created));
          var modifiedProperty = collectionType.GetProperty(nameof(BaseEntity.Modified));

          idProperty?.SetValue(obj: item,
                               value: Guid.NewGuid());
          createdProperty?.SetValue(obj: item,
                                    value: DateTime.UtcNow);
          modifiedProperty?.SetValue(obj: item,
                                     value: DateTime.UtcNow);

          if (isEnumPortProperty)
          {
            // Преобразование строки в значение enum
            var enumValue = Enum.Parse(enumType: portProperty.PropertyType,
                                       value: valueToSet[i]);
            portProperty.SetValue(obj: item,
                                  value: enumValue);
          }
          else
          {
            // Конвертация строки в значение нужного типа
            var convertedValue = Convert.ChangeType(value: valueToSet[i],
                                                    conversionType: portProperty.PropertyType);
            portProperty.SetValue(obj: item,
                                  value: convertedValue);

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
          var enumValue = Enum.Parse(enumType: portProperty.PropertyType,
                                     value: valueToSet[i]);
          portProperty.SetValue(obj: item,
                                value: enumValue);
        }
        else
        {
          // Конвертация и установка нового значения
          var convertedValue = Convert.ChangeType(value: valueToSet[i],
                                                  conversionType: portProperty.PropertyType);
          portProperty.SetValue(obj: item,
                                value: convertedValue);
        }
      }
    }
  }

  private async Task FillARPTableForPorts(NetworkDevice networkDevice,
                                          string host,
                                          string community,
                                          CancellationToken cancellationToken)
  {
    // Выполняем SNMP-запросы
    var interfaceNumbers = await _SNMPCommandExecutor.WalkCommand(host: host,
                                                                  community: community,
                                                                  oid: "1.3.6.1.2.1.4.22.1.1",
                                                                  cancellationToken: cancellationToken);
    var macAddresses = await _SNMPCommandExecutor.WalkCommand(host: host,
                                                              community: community,
                                                              oid: "1.3.6.1.2.1.4.22.1.2",
                                                              cancellationToken);
    var ipAddresses = await _SNMPCommandExecutor.WalkCommand(host: host,
                                                             community: community,
                                                             oid: "1.3.6.1.2.1.4.22.1.3",
                                                             cancellationToken);

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

    var arpDictionary = arpEntries.GroupBy(entry => int.Parse(entry.Interface))
                                  .ToDictionary(keySelector: group => group.Key,
                                                elementSelector: group => group.Select(e => new KeyValuePair<string, string>(e.Mac,
                                                                                                                             e.Ip))
                                  .ToList());


    // Заполнение ARP таблицы для каждого порта
    foreach (var port in networkDevice.PortsOfNetworkDevice)
    {
      // Проверяем, есть ли ARP записи для текущего порта в словаре arpDictionary
      if (arpDictionary.TryGetValue(port.InterfaceNumber, out var arpForPort))
      {
        // Создаем новую таблицу, которая поддерживает несколько IP для одного MAC
        var arpTable = new Dictionary<string, HashSet<string>>();

        // Проходим по каждой записи и добавляем в таблицу
        foreach (var arpEntry in arpForPort)
        {
          // Если MAC-адрес уже существует в таблице, добавляем IP в список
          if (!arpTable.TryGetValue(arpEntry.Key, out HashSet<string>? value))
          {
            value = ([]);
            arpTable[arpEntry.Key] = value;
          }

          value.Add(arpEntry.Value);
        }

        // Присваиваем заполненную ARP таблицу порту
        port.ARPTableOfPort = arpTable;
      }
    }
  }

  private async Task FillNetworkTableOfPort(NetworkDevice networkDevice,
                                            string host,
                                            string community,
                                            CancellationToken cancellationToken)
  {
    // Выполняем SNMP-запрос для получения номеров интерфейсов
    var interfaceNumbers = await _SNMPCommandExecutor.WalkCommand(host: host,
                                                                  community: community,
                                                                  oid: "1.3.6.1.2.1.4.20.1.2",
                                                                  cancellationToken: cancellationToken);
    // Выполняем SNMP-запрос для получения IP-адресов
    var ipAddresses = await _SNMPCommandExecutor.WalkCommand(host: host,
                                                             community: community,
                                                             oid: "1.3.6.1.2.1.4.20.1.1",
                                                             cancellationToken);
    // Выполняем SNMP-запрос для получения сетевых масок
    var netMasks = await _SNMPCommandExecutor.WalkCommand(host: host,
                                                          community: community,
                                                          oid: "1.3.6.1.2.1.4.20.1.3",
                                                          cancellationToken);

    // Проверяем, что хотя бы один из запросов не вернул пустые данные
    if (interfaceNumbers.Count == 0 || ipAddresses.Count == 0 || ipAddresses.Count == 0)
      throw new InvalidOperationException("One from SNMP requests receive empty result.");

    // Проверяем, что количество элементов в каждом запросе совпадает
    if (interfaceNumbers.Count != ipAddresses.Count || ipAddresses.Count != ipAddresses.Count)
      throw new InvalidOperationException($"SNMP responses count mismatch for ARP data: interfaces({interfaceNumbers.Count}), macs({ipAddresses.Count}), ips({ipAddresses.Count})");

    // Объединяем результаты SNMP-запросов: interfaceNumber -> IP -> Mask
    var ipTableEntries = interfaceNumbers.Zip(second: ipAddresses,
                                              resultSelector: (iface, address) => new
                                              {
                                                iface,
                                                address
                                              })
                                          .Zip(second: netMasks,
                                               resultSelector: (firstPair, mask) => new
                                               {
                                                 Interface = firstPair.iface.Data,
                                                 Address = firstPair.address.Data,
                                                 Mask = mask.Data
                                               });

    // Группируем данные по номерам интерфейсов и создаем словарь
    var arpDictionary = ipTableEntries.GroupBy(entry => int.Parse(entry.Interface))
                                      .ToDictionary(keySelector: group => group.Key,
                                                    elementSelector: group => group.Select(e => new KeyValuePair<string, string>(e.Address,
                                                                                                                                 e.Mask))
                                      .ToList());

    // Заполняем сетевую таблицу для каждого порта устройства
    foreach (var port in networkDevice.PortsOfNetworkDevice)
    {
      // Если есть записи для порта, то создаем сетевую таблицу
      if (arpDictionary.TryGetValue(port.InterfaceNumber, out var arpForPort))
      {
        var arpTable = new Dictionary<string, string>();

        // Добавляем записи IP и масок в сетевую таблицу порта
        foreach (var arpEntry in arpForPort)
        {
          arpTable.Add(key: arpEntry.Key, value: arpEntry.Value);
        }
        // Присваиваем заполненную сетевую таблицу порту
        port.NetworkTableOfPort = arpTable;
      }
    }
  }
}