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

    var networkDevice = new NetworkDevice
    {
      NetworkDeviceName = request.NetworkDeviceName
    };
    var assignments = await _assignmentRepository.GetAllAsync(cancellationToken);
    var businessRules = await _businessRulesRepository.GetAllAsync(cancellationToken);

    foreach (var businessRule in businessRules.DistinctBy(x => x.Created))
    {
      bool result = true;

      if (businessRule.Condition is not null)
        result = await businessRule.EvaluateConditionAsync(networkDevice);

      if (result)
      {
        var assigment = assignments.FirstOrDefault(x => x.Id == businessRule.AssignmentId);
        var valueToSet = await _SNMPCommandExecutor.GetCommand(host: request.Host,
                                                               community: request.Community,
                                                               oid: assigment!.OID,
                                                               cancellationToken: cancellationToken);
        _ = HandleAssignmentAsync(networkDevice, assigment, valueToSet.Data);
        // TODO work 1
      }
      else
      {
        // TODO work 2
      }
    }

    await _networkDeviceRepository.InsertOneAsync(networkDevice,
                                                  cancellationToken);

    return Unit.Value;
  }
  public static async Task HandleAssignmentAsync(NetworkDevice networkDevice, Assignment assignment, string valueToSet)
  {
    if (assignment == null || string.IsNullOrWhiteSpace(assignment.TargetFieldName))
    {
      throw new ArgumentException("Assignment or TargetFieldName cannot be null or empty.");
    }

    // Получаем тип NetworkDevice
    var deviceType = typeof(NetworkDevice);

    // Получаем свойство по имени
    var property = deviceType.GetProperty(assignment.TargetFieldName, BindingFlags.Public | BindingFlags.Instance);

    if (property == null)
    {
      throw new InvalidOperationException($"Property '{assignment.TargetFieldName}' not found on {deviceType.Name}.");
    }

    // Устанавливаем значение свойства
    // Конвертируем значение в нужный тип
    var convertedValue = Convert.ChangeType(valueToSet, property.PropertyType);
    property.SetValue(networkDevice, convertedValue);

    // Дополнительные действия после установки значения, если необходимо
    await Task.CompletedTask;
  }
}