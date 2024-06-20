namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices;

public record CreateNetworkDeviceCommand(IPAddress Host) : ICommand;

internal class CreateNetworkDeviceCommandHandler(INetworkDeviceRepository networkDeviceRepository,
                                                 ISNMPRepository sNMPRepository)
  : ICommandHandler<CreateNetworkDeviceCommand, Unit>
{
  readonly INetworkDeviceRepository _networkDeviceRepository = networkDeviceRepository
    ?? throw new ArgumentNullException(nameof(networkDeviceRepository));
  readonly ISNMPRepository _sNMPRepository = sNMPRepository
    ?? throw new ArgumentNullException(nameof(sNMPRepository));

  async Task<Unit> IRequestHandler<CreateNetworkDeviceCommand, Unit>.Handle(CreateNetworkDeviceCommand request,
                                                                            CancellationToken cancellationToken)
  {
    var existsGateway = await _networkDeviceRepository.ExistsAsync(x => x.Host == request.Host,
                                                                   cancellationToken);

    if (existsGateway)
      throw new NetworkDeviceAlreadyExists($"Network device \"{request.Host}\" already exists.");

    var networkDevice = new NetworkDevice()
    {

    };

    await _networkDeviceRepository.InsertOneAsync(networkDevice, cancellationToken);

    return Unit.Value;
  }
}
