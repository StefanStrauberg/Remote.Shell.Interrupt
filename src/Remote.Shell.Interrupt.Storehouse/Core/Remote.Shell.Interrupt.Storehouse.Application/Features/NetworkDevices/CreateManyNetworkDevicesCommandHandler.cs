namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices;

public record CreateManyNetworkDevicesCommand(IEnumerable<IPAddress> Hosts, bool Replace) : ICommand;

public class CreateManyNetworkDevicesCommandHandler(INetworkDeviceRepository networkDeviceRepository,
                                                    IBusinessRuleRepository businessRulesRepository)
  : ICommandHandler<CreateManyNetworkDevicesCommand, Unit>
{
  readonly INetworkDeviceRepository _networkDeviceRepository = networkDeviceRepository
    ?? throw new ArgumentNullException(nameof(networkDeviceRepository));
  readonly IBusinessRuleRepository _businessRulesRepository = businessRulesRepository
    ?? throw new ArgumentNullException(nameof(businessRulesRepository));

  async Task<Unit> IRequestHandler<CreateManyNetworkDevicesCommand, Unit>.Handle(CreateManyNetworkDevicesCommand request,
                                                                                 CancellationToken cancellationToken)
  {
    IEnumerable<NetworkDevice> networkDevices = [];

    foreach (var host in request.Hosts)
    {
      var existingNetworkDevice = await _networkDeviceRepository.ExistsAsync(x => x.Host == host,
                                                                             cancellationToken);

      // If a Network Device exists and the Replace is false
      // throw EntityAlreadyExists exception
      if (existingNetworkDevice && !request.Replace)
        throw new EntityAlreadyExists(host.ToString());

      var networkDevice = new NetworkDevice()
      {
        // ToDo Business logic
      };
    }

    await _networkDeviceRepository.InsertManyAsync(networkDevices,
                                                   cancellationToken);
    return Unit.Value;
  }
}
