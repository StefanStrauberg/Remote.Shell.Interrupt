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
    var filterNetworkDevice = (Expression<Func<NetworkDevice, bool>>)(x => x.Host == IPAddress.Parse(request.Host));
    var existingNetworkDevice = await _networkDeviceRepository.ExistsAsync(filterExpression: filterNetworkDevice,
                                                                           cancellationToken: cancellationToken);

    if (existingNetworkDevice)
      throw new EntityAlreadyExists(request.Host.ToString());

    var networkDevice = new NetworkDevice
    {
      NetworkDeviceName = request.NetworkDeviceName,
      Host = IPAddress.Parse(request.Host)
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
    // Evaluate the condition for the current rule
    bool resultOfEvaluateCondition = rule.Condition == null || await rule.EvaluateConditionAsync(networkDevice);

    if (resultOfEvaluateCondition)
    {
      if (rule.AssignmentId.HasValue)
      {
        var assignment = assignments.Single(x => x.Id == rule.AssignmentId.Value);

        switch (assignment.TypeOfRequest)
        {
          case TypeOfRequest.get:
            var singleValueToSet = (await _SNMPCommandExecutor.GetCommand(request.Host, request.Community, assignment.OID, cancellationToken)).Data;
            HandleAssignment(networkDevice,
                             assignment,
                             singleValueToSet);
            break;
          case TypeOfRequest.walk:
            var multiplyValuesToSet = (await _SNMPCommandExecutor.WalkCommand(request.Host, request.Community, assignment.OID, cancellationToken))
                .Select(x => x.Data)
                .ToList();
            HandleAssignment(networkDevice,
                             assignment,
                             multiplyValuesToSet);
            break;
        }
      }
    }

    // Recursively process child rules
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
    if (assignment == null || string.IsNullOrWhiteSpace(assignment.TargetFieldName))
      throw new ArgumentException("Assignment or TargetFieldName cannot be null or empty.");

    // Get type of NetworkDevice
    var deviceType = typeof(NetworkDevice);

    // Get properties by names
    var property = deviceType.GetProperty(name: assignment.TargetFieldName,
                                          bindingAttr: BindingFlags.Public | BindingFlags.Instance)
      ?? throw new InvalidOperationException($"Property '{assignment.TargetFieldName}' not found on {deviceType.Name}.");

    // Set values to properties and
    // Convert values to necessary types
    var convertedValue = Convert.ChangeType(value: valueToSet,
                                            conversionType: property.PropertyType);
    property.SetValue(obj: networkDevice,
                      value: convertedValue);
  }

  static void HandleAssignment(NetworkDevice networkDevice,
                               Assignment assignment,
                               List<string> valueToSet)
  {
    if (assignment == null || string.IsNullOrWhiteSpace(assignment.TargetFieldName))
      throw new ArgumentException("Assignment or TargetFieldName cannot be null or empty.");

    var ports = networkDevice.PortsOfNetworkDevice.Count == 0 ? new List<Port>(valueToSet.Count) : networkDevice.PortsOfNetworkDevice;

    var portType = typeof(Port);
    PropertyInfo property;

    // Handle multiple value assignment
    switch (assignment.TargetFieldName)
    {
      case "InterfaceNumber":
        property = portType.GetProperty(name: assignment.TargetFieldName,
                                        bindingAttr: BindingFlags.Public | BindingFlags.Instance)
          ?? throw new InvalidOperationException($"Property '{assignment.TargetFieldName}' not found on {portType.Name}.");

        if (ports.Count == 0)
        {
          foreach (var value in valueToSet)
          {
            var port = new Port
            {
              Id = new Guid(),
              Created = DateTime.UtcNow,
              Modified = DateTime.UtcNow,
            };

            ports.Add(port);
            // Set values to properties and
            // Convert values to necessary types
            var convertedValue = Convert.ChangeType(value: value,
                                                    conversionType: property.PropertyType);
            property.SetValue(obj: port,
                              value: convertedValue);
          }
        }
        else
        {
          for (int i = 0; i < ports.Count; i++)
          {
            // Set values to properties and
            // Convert values to necessary types
            var convertedValue = Convert.ChangeType(value: valueToSet[i],
                                                    conversionType: property.PropertyType);
            property.SetValue(obj: ports[i],
                              value: convertedValue);
          }
        }
        networkDevice.PortsOfNetworkDevice = ports;
        break;

      case "PortName":
        property = portType.GetProperty(name: assignment.TargetFieldName,
                                        bindingAttr: BindingFlags.Public | BindingFlags.Instance)
          ?? throw new InvalidOperationException($"Property '{assignment.TargetFieldName}' not found on {portType.Name}.");

        if (ports.Count == 0)
        {
          foreach (var value in valueToSet)
          {
            var port = new Port
            {
              Id = new Guid(),
              Created = DateTime.UtcNow,
              Modified = DateTime.UtcNow,
            };

            ports.Add(port);
            // Set values to properties and
            // Convert values to necessary types
            var convertedValue = Convert.ChangeType(value: value,
                                                    conversionType: property.PropertyType);
            property.SetValue(obj: port,
                              value: convertedValue);
          }
        }
        else
        {
          for (int i = 0; i < ports.Count; i++)
          {
            // Set values to properties and
            // Convert values to necessary types
            var convertedValue = Convert.ChangeType(value: valueToSet[i],
                                                    conversionType: property.PropertyType);
            property.SetValue(obj: ports[i],
                              value: convertedValue);
          }
        }
        networkDevice.PortsOfNetworkDevice = ports;
        break;

      case "InterfaceType":
        property = portType.GetProperty(name: assignment.TargetFieldName,
                                        bindingAttr: BindingFlags.Public | BindingFlags.Instance)
          ?? throw new InvalidOperationException($"Property '{assignment.TargetFieldName}' not found on {portType.Name}.");

        if (ports.Count == 0)
        {
          foreach (var value in valueToSet)
          {
            var port = new Port
            {
              Id = new Guid(),
              Created = DateTime.UtcNow,
              Modified = DateTime.UtcNow,
            };

            ports.Add(port);
            // Set values to properties and
            // Convert values to necessary types
            var convertedValue = Convert.ChangeType(value: value,
                                                    conversionType: property.PropertyType);
            property.SetValue(obj: port,
                              value: convertedValue);
          }
        }
        else
        {
          for (int i = 0; i < ports.Count; i++)
          {
            // Set values to properties and
            // Convert values to necessary types
            var convertedValue = Convert.ChangeType(value: valueToSet[i],
                                                    conversionType: property.PropertyType);
            property.SetValue(obj: ports[i],
                              value: convertedValue);
          }
        }
        networkDevice.PortsOfNetworkDevice = ports;
        break;

      case "SpeedOfPort":
        property = portType.GetProperty(name: assignment.TargetFieldName,
                                        bindingAttr: BindingFlags.Public | BindingFlags.Instance)
          ?? throw new InvalidOperationException($"Property '{assignment.TargetFieldName}' not found on {portType.Name}.");

        if (ports.Count == 0)
        {
          foreach (var value in valueToSet)
          {
            var port = new Port
            {
              Id = new Guid(),
              Created = DateTime.UtcNow,
              Modified = DateTime.UtcNow,
            };

            ports.Add(port);
            // Set values to properties and
            // Convert values to necessary types
            var convertedValue = Convert.ChangeType(value: value,
                                                    conversionType: property.PropertyType);
            property.SetValue(obj: port,
                              value: convertedValue);
          }
        }
        else
        {
          for (int i = 0; i < ports.Count; i++)
          {
            // Set values to properties and
            // Convert values to necessary types
            var convertedValue = Convert.ChangeType(value: valueToSet[i],
                                                    conversionType: property.PropertyType);
            property.SetValue(obj: ports[i],
                              value: convertedValue);
          }
        }
        networkDevice.PortsOfNetworkDevice = ports;
        break;

      // Add additional cases as needed for other collections
      default:
        throw new InvalidOperationException($"Unhandled TargetFieldName '{assignment.TargetFieldName}' for collection processing.");
    }
  }
}