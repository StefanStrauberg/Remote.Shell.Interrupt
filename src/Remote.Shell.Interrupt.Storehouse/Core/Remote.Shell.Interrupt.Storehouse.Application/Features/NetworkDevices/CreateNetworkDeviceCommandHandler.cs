namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices;

public record CreateNetworkDeviceCommand(IPAddress Host) : ICommand;

internal class CreateNetworkDeviceCommandHandler(INetworkDeviceRepository networkDeviceRepository,
                                                 IGatewayRepository gatewayRepository)
  : ICommandHandler<CreateNetworkDeviceCommand, Unit>
{
  readonly INetworkDeviceRepository _networkDeviceRepository = networkDeviceRepository
    ?? throw new ArgumentNullException(nameof(networkDeviceRepository));
  readonly IGatewayRepository _gatewayRepository = gatewayRepository
    ?? throw new ArgumentNullException(nameof(gatewayRepository));

  async Task<Unit> IRequestHandler<CreateNetworkDeviceCommand, Unit>.Handle(CreateNetworkDeviceCommand request,
                                                                            CancellationToken cancellationToken)
  {
    var gateway = await _gatewayRepository.ExistsAsync(x => x.Host == request.Host,
                                                       cancellationToken);

    if (gateway)
      throw new NetworkDeviceExists($"Network device \"{request.Host}\" already exists.");

    var networkDevice = new NetworkDevice()
    {

    };

    await _networkDeviceRepository.InsertOneAsync(networkDevice, cancellationToken);

    return Unit.Value;
  }
}
