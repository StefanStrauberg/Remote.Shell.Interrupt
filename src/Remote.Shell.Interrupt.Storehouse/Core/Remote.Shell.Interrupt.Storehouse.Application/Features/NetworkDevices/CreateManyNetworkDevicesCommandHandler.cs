namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices;

public record CreateManyNetworkDevicesCommand(IEnumerable<IPAddress> Hosts, bool Replace) : ICommand;

public class CreateManyNetworkDevicesCommandHandler(INetworkDeviceRepository networkDeviceRepository)
  : ICommandHandler<CreateManyNetworkDevicesCommand, Unit>
{
  readonly INetworkDeviceRepository _networkDeviceRepository = networkDeviceRepository
    ?? throw new ArgumentNullException(nameof(networkDeviceRepository));

  async Task<Unit> IRequestHandler<CreateManyNetworkDevicesCommand, Unit>.Handle(CreateManyNetworkDevicesCommand request,
                                                                                 CancellationToken cancellationToken)
  {
    IEnumerable<NetworkDevice> networkDevices = [];

    foreach (var host in request.Hosts)
    {
      var existingGateway = await _networkDeviceRepository.ExistsAsync(x => x.Host == host,
                                                                       cancellationToken);

      if (existingGateway && !request.Replace)
        continue; // If Network Device exists we skip it

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
