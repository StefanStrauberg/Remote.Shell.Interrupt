namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices;

public record CreateNetworkDeviceCommand(IPAddress Host) : ICommand;

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

    // If a Network Device exists throw EntityAlreadyExists exception
    if (existingNetworkDevice)
      throw new EntityAlreadyExists(request.Host.ToString());

    var networkDevice = new NetworkDevice() { };
    var assignments = await _assignmentRepository.GetAllAsync(cancellationToken);
    var businessRules = await _businessRulesRepository.GetAllAsync(cancellationToken);

    foreach (var businessRule in businessRules)
    {
      bool result = await businessRule.EvaluateConditionAsync(networkDevice);

      if (result)
      {
        // TODO work
      }
      else
      {
        // TODO work
      }
    }

    await _networkDeviceRepository.InsertOneAsync(networkDevice,
                                                  cancellationToken);

    return Unit.Value;
  }
}
