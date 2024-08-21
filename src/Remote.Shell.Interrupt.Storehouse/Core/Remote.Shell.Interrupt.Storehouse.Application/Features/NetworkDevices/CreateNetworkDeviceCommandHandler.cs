namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices;

public record CreateNetworkDeviceCommand(IPAddress Host, bool Replace) : ICommand;

internal class CreateNetworkDeviceCommandHandler(INetworkDeviceRepository networkDeviceRepository,
                                                 IBusinessRuleRepository businessRulesRepository)
  : ICommandHandler<CreateNetworkDeviceCommand, Unit>
{
  readonly INetworkDeviceRepository _networkDeviceRepository = networkDeviceRepository
    ?? throw new ArgumentNullException(nameof(networkDeviceRepository));
  readonly IBusinessRuleRepository _businessRulesRepository = businessRulesRepository
    ?? throw new ArgumentNullException(nameof(businessRulesRepository));

  async Task<Unit> IRequestHandler<CreateNetworkDeviceCommand, Unit>.Handle(CreateNetworkDeviceCommand request,
                                                                            CancellationToken cancellationToken)
  {
    var existingNetworkDevice = await _networkDeviceRepository.ExistsAsync(filterExpression: x => x.Host == request.Host,
                                                                           cancellationToken: cancellationToken);
    var businessRules = await _businessRulesRepository.GetAllAsync(cancellationToken);

    // If a Network Device exists and the Replace is false
    // throw EntityAlreadyExists exception
    if (existingNetworkDevice && !request.Replace)
      throw new EntityAlreadyExists(request.Host.ToString());

    var networkDevice = new NetworkDevice() { };

    foreach (var businessRule in businessRules)
    {
      // TODO: implement business rule logic
    }

    await _networkDeviceRepository.InsertOneAsync(networkDevice, cancellationToken);

    return Unit.Value;
  }
}
