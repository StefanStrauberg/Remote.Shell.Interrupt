namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices;

public record CreateNetworkDeviceCommand(IPAddress Host, bool Replace) : ICommand;

internal class CreateNetworkDeviceCommandHandler(INetworkDeviceRepository networkDeviceRepository)
  : ICommandHandler<CreateNetworkDeviceCommand, Unit>
{
  readonly INetworkDeviceRepository _networkDeviceRepository = networkDeviceRepository
    ?? throw new ArgumentNullException(nameof(networkDeviceRepository));

  async Task<Unit> IRequestHandler<CreateNetworkDeviceCommand, Unit>.Handle(CreateNetworkDeviceCommand request,
                                                                            CancellationToken cancellationToken)
  {
    var existingGateway = await _networkDeviceRepository.ExistsAsync(x => x.Host == request.Host,
                                                                     cancellationToken);

    if (existingGateway && !request.Replace)
      throw new EntityAlreadyExists(request.Host.ToString());

    var networkDevice = new NetworkDevice()
    {

    };

    await _networkDeviceRepository.InsertOneAsync(networkDevice, cancellationToken);

    return Unit.Value;
  }
}
