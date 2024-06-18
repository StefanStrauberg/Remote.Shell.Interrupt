namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices;

public record CreateNetworkDeviceCommand(IPAddress Host,
                                         string Vendor,
                                         string Model,
                                         string SoftwareVersion,
                                         GatewayLevel GatewayLevel,
                                         ICollection<Interface> Interfaces,
                                         ICollection<VLAN> VLANs,
                                         ICollection<ARPEntry> ARPTable) : ICommand;

internal class CreateNetworkDeviceCommandHandler(INetworkDeviceRepository networkDeviceRepository)
  : ICommandHandler<CreateNetworkDeviceCommand, Unit>
{
  readonly INetworkDeviceRepository _networkDeviceRepository = networkDeviceRepository
    ?? throw new ArgumentNullException(nameof(networkDeviceRepository));

  async Task<Unit> IRequestHandler<CreateNetworkDeviceCommand, Unit>.Handle(CreateNetworkDeviceCommand request,
                                                                            CancellationToken cancellationToken)
  {
    var createNetworkDevice = request.Adapt<NetworkDevice>();

    await _networkDeviceRepository.InsertOneAsync(createNetworkDevice,
                                                  cancellationToken);
    return Unit.Value;
  }
}
