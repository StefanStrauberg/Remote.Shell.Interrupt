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
    var existingNetworkDevice = await _networkDeviceRepository.ExistsAsync(filterExpression: x => x.Host == request.Host,
                                                                           cancellationToken: cancellationToken);

    // If a Network Device exists throw EntityAlreadyExists exception
    if (existingNetworkDevice)
      throw new EntityAlreadyExists(request.Host.ToString());

    var businessRules = await _businessRulesRepository.GetAllAsync(cancellationToken);
    var networkDevice = new NetworkDevice() { };

    // foreach (var businessRule in businessRules)
    // {
    //   // TODO: implement business rule logic
    //   if (condifion)
    //   {

    //   }
    // }

    await _networkDeviceRepository.InsertOneAsync(networkDevice, cancellationToken);

    return Unit.Value;
  }
}
