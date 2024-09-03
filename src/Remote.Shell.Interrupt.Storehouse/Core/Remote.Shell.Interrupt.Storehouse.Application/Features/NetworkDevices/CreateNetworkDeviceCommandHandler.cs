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

    // If a Network Device exists throw EntityAlreadyExists exception
    if (existingNetworkDevice)
      throw new EntityAlreadyExists(request.Host.ToString());

    // Create new instance of NetworkDevice and set NetworkDeviceName property of it
    var networkDevice = new NetworkDevice
    {
      NetworkDeviceName = request.NetworkDeviceName,
      Host = IPAddress.Parse(request.Host)
    };
    // Get all assignments and business rules
    var assignments = await _assignmentRepository.GetAllAsync(cancellationToken);
    var businessRules = await _businessRulesRepository.GetAllAsync(cancellationToken);

    // walktrhough business rules
    foreach (var businessRule in businessRules.DistinctBy(x => x.Created))
    {
      // default value of EvaluateCondition is true
      bool resultOfEvaluateCondition = true;

      if (businessRule.Condition is not null)
        resultOfEvaluateCondition = await businessRule.EvaluateConditionAsync(networkDevice);

      if (resultOfEvaluateCondition)
      {
        var assigment = assignments.Single(x => x.Id == businessRule.AssignmentId);

        switch (assigment.TypeOfRequest)
        {
          case TypeOfRequest.get:
            var singleValueToSet = (await _SNMPCommandExecutor.GetCommand(host: request.Host,
                                                                          community: request.Community,
                                                                          oid: assigment!.OID,
                                                                          cancellationToken: cancellationToken)).Data;
            HandleAssignment(networkDevice: networkDevice,
                             assignment: assigment,
                             valueToSet: singleValueToSet);
            break;
          case TypeOfRequest.walk:
            var multiplyValuesToSet = (await _SNMPCommandExecutor.WalkCommand(host: request.Host,
                                                                              community: request.Community,
                                                                              oid: assigment!.OID,
                                                                              cancellationToken: cancellationToken))
                                                                 .Select(x => x.Data)
                                                                 .ToList();
            HandleAssignment(networkDevice: networkDevice,
                             assignment: assigment,
                             valueToSet: multiplyValuesToSet);
            break;
        }
      }
      else
        continue;
    }

    await _networkDeviceRepository.InsertOneAsync(networkDevice,
                                                  cancellationToken);

    return Unit.Value;
  }
  public static void HandleAssignment(NetworkDevice networkDevice,
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

  public static void HandleAssignment(NetworkDevice networkDevice,
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