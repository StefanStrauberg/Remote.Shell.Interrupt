namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices;

public record CreateManyNetworkDevicesCommand(IEnumerable<IPAddress> Hosts) : ICommand;

public class CreateManyNetworkDevicesCommandHandler(INetworkDeviceRepository networkDeviceRepository)
  : ICommandHandler<CreateManyNetworkDevicesCommand, Unit>
{
  readonly INetworkDeviceRepository _networkDeviceRepository = networkDeviceRepository
    ?? throw new ArgumentNullException(nameof(networkDeviceRepository));

  async Task<Unit> IRequestHandler<CreateManyNetworkDevicesCommand, Unit>.Handle(CreateManyNetworkDevicesCommand request,
                                                                                 CancellationToken cancellationToken)
  {
    var networkDevices = request.Adapt<IEnumerable<NetworkDevice>>();

    await _networkDeviceRepository.InsertManyAsync(networkDevices,
                                                   cancellationToken);
    return Unit.Value;
  }
}
