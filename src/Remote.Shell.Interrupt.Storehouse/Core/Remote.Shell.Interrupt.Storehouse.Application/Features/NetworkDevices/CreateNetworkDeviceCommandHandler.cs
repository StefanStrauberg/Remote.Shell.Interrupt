namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices;

public record CreateNetworkDeviceCommand(IPAddress Host, bool Replace) : ICommand;

internal class CreateNetworkDeviceCommandHandler(INetworkDeviceRepository networkDeviceRepository,
                                                 ICollection<BusinessRule> businessRules)
  : ICommandHandler<CreateNetworkDeviceCommand, Unit>
{
  readonly INetworkDeviceRepository _networkDeviceRepository = networkDeviceRepository
    ?? throw new ArgumentNullException(nameof(networkDeviceRepository));
  readonly ICollection<BusinessRule> _businessRules = businessRules
    ?? throw new ArgumentNullException(nameof(businessRules));

  async Task<Unit> IRequestHandler<CreateNetworkDeviceCommand, Unit>.Handle(CreateNetworkDeviceCommand request,
                                                                            CancellationToken cancellationToken)
  {
    var existingGateway = await _networkDeviceRepository.ExistsAsync(x => x.Host == request.Host,
                                                                     cancellationToken);

    if (existingGateway && !request.Replace)
      throw new EntityAlreadyExists(request.Host.ToString());

    var networkDevice = new NetworkDevice() { };

    foreach (var item in _businessRules)
    {
      System.Console.WriteLine(item.RuleName);
      // ToDo business logic with networkDevice
      // ToDo implement OID request
      if (item.Condition != null)
      {
        // Compile the expression into a delegate
        var conditionFunc = item.Condition.Compile();

        // Evaluate the condition with the given parameter
        bool conditionResult = conditionFunc(networkDevice);

        if (conditionResult)
        {
          // Implement your business logic here
          Console.WriteLine($"Condition met for rule: {item.RuleName}");
        }
      }
    }

    await _networkDeviceRepository.InsertOneAsync(networkDevice, cancellationToken);

    return Unit.Value;
  }
}
