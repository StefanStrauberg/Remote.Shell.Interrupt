namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices;

public record CreateNetworkDeviceCommand(string Host, string Community, string NetworkDeviceName) : ICommand;

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
    var filterNetworkDevice = (Expression<Func<NetworkDevice, bool>>)(x => x.Host == request.Host);
    var existingNetworkDevice = await _networkDeviceRepository.ExistsAsync(filterExpression: filterNetworkDevice,
                                                                           cancellationToken: cancellationToken);

    if (existingNetworkDevice)
      throw new EntityAlreadyExists(request.Host);

    var networkDevice = new NetworkDevice
    {
      NetworkDeviceName = request.NetworkDeviceName,
      Host = request.Host,
      Created = DateTime.UtcNow,
      Modified = DateTime.UtcNow
    };

    var assignments = await _assignmentRepository.GetAllAsync(cancellationToken);
    var businessRules = await _businessRulesRepository.GetAllAsync(cancellationToken);

    if (!businessRules.Any())
      throw new InvalidOperationException($"Busness Rules collection is empty.");

    var rootRule = businessRules.FirstOrDefault(x => x.IsRoot == true)
      ?? throw new InvalidOperationException($"Busness Rules collection doesn't have root element.");

    await ProcessBusinessRuleTree(rootRule,
                                  businessRules,
                                  assignments,
                                  networkDevice,
                                  request,
                                  cancellationToken);

    await _networkDeviceRepository.InsertOneAsync(networkDevice,
                                                  cancellationToken);
    return Unit.Value;
  }
  async Task ProcessBusinessRuleTree(BusinessRule rule,
                                     IEnumerable<BusinessRule> allRules,
                                     IEnumerable<Assignment> assignments,
                                     NetworkDevice networkDevice,
                                     CreateNetworkDeviceCommand request,
                                     CancellationToken cancellationToken)
  {
    bool resultOfEvaluateCondition = rule.Condition == null || await rule.EvaluateConditionAsync(networkDevice);

    if (resultOfEvaluateCondition)
    {
      if (rule.AssignmentId.HasValue)
      {
        var assignment = assignments.Single(x => x.Id == rule.AssignmentId.Value);

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

  static void HandleAssignment(NetworkDevice networkDevice,
                               Assignment assignment,
                               string valueToSet)
  {
    // Проверка TargetFieldName является ли null или пустая строка
    if (assignment == null || string.IsNullOrWhiteSpace(assignment.TargetFieldName))
      throw new ArgumentException("Assignment or TargetFieldName cannot be null or empty.");

    // Получение типа NetworkDevice
    var deviceType = typeof(NetworkDevice);

    // Получение свойства по имени
    var property = deviceType.GetProperty(name: assignment.TargetFieldName,
                                          bindingAttr: BindingFlags.Public | BindingFlags.Instance)
      ?? throw new InvalidOperationException($"Property '{assignment.TargetFieldName}' not found on {deviceType.Name}.");

    // Конвертация устанавливоемого значения в необходимый тип и установка свойства
    var convertedValue = Convert.ChangeType(value: valueToSet,
                                            conversionType: property.PropertyType);
    property.SetValue(obj: networkDevice,
                      value: convertedValue);
  }

  static void HandleAssignment(NetworkDevice networkDevice,
                               Assignment assignment,
                               List<string> valueToSet)
  {
    // Проверка TargetFieldName
    if (assignment == null || string.IsNullOrWhiteSpace(assignment.TargetFieldName))
      throw new ArgumentException("Assignment or TargetFieldName cannot be null or empty.");

    // Получение типа NetworkDevice и Port
    var networkDeviceType = typeof(NetworkDevice);

    // Разделение имени свойства на часть для NetworkDevice и Port
    var parts = assignment.TargetFieldName.Split('.');
    if (parts.Length != 2)
      throw new ArgumentException("TargetFieldName must be in the format 'Property.Field'.");

    var networkDeviceFieldName = parts[0];
    var portFieldName = parts[1];

    // Проверка существования свойства на NetworkDevice
    PropertyInfo portsProperty = networkDeviceType.GetProperty(networkDeviceFieldName,
                                                               BindingFlags.Public | BindingFlags.Instance)!
      ?? throw new ArgumentException($"Property '{networkDeviceFieldName}' not found on {networkDeviceType.Name}.");

    // Проверка, является ли свойство коллекцией
    if (!typeof(IEnumerable).IsAssignableFrom(portsProperty.PropertyType) || portsProperty.PropertyType == typeof(string))
      throw new ArgumentException($"Property '{networkDeviceFieldName}' is not a collection.");

    // Получение типа элементов коллекции
    Type collectionType = portsProperty.PropertyType
                                       .GetGenericArguments()
                                       .FirstOrDefault()
      ?? throw new ArgumentException("Cannot determine collection item type.");

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

    // Обновление коллекции
    if (collection.Count == 0)
    {
      for (int i = 0; i < valueToSet.Count; i++)
      {
        // Создание и добавление элементов в коллекцию
        var item = Activator.CreateInstance(collectionType);

        if (item != null)
        {
          var idProperty = collectionType.GetProperty(nameof(BaseEntity.Id));
          var createdProperty = collectionType.GetProperty(nameof(BaseEntity.Created));
          var modifiedProperty = collectionType.GetProperty(nameof(BaseEntity.Modified));

          idProperty?.SetValue(item, Guid.NewGuid());
          createdProperty?.SetValue(item, DateTime.UtcNow);
          modifiedProperty?.SetValue(item, DateTime.UtcNow);

          if (isEnumPortProperty)
          {
            // Преобразование строки в значение enum
            var enumValue = Enum.Parse(portProperty.PropertyType,
                                       valueToSet[i]);
            portProperty.SetValue(item,
                                  enumValue);
          }
          else
          {

            // Установка значения свойства
            var convertedValue = Convert.ChangeType(valueToSet[i],
                                                    portProperty.PropertyType);
            portProperty.SetValue(item,
                                  convertedValue);

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
          var enumValue = Enum.Parse(portProperty.PropertyType, valueToSet[i]);
          portProperty.SetValue(item, enumValue);
        }
        else
        {
          var convertedValue = Convert.ChangeType(valueToSet[i], portProperty.PropertyType);
          portProperty.SetValue(item, convertedValue);
        }
      }
    }
  }
}