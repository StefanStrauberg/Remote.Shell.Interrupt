
namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices;

public record UpdateNetworkDeviceCommand(Guid Id,
                                         IPAddress Host,
                                         string Vendor,
                                         string Model,
                                         string SoftwareVersion,
                                         GatewayLevel GatewayLevel,
                                         ICollection<Interface> Interfaces,
                                         ICollection<VLAN> VLANs,
                                         ICollection<ARPEntry> ARPTable) : ICommand;

internal class UpdateNetworkDeviceCommandHandler(INetworkDeviceRepository networkDeviceRepository)
  : ICommandHandler<UpdateNetworkDeviceCommand, Unit>
{
  readonly INetworkDeviceRepository _networkDeviceRepository = networkDeviceRepository
    ?? throw new ArgumentNullException(nameof(networkDeviceRepository));

  async Task<Unit> IRequestHandler<UpdateNetworkDeviceCommand, Unit>.Handle(UpdateNetworkDeviceCommand request,
                                                                            CancellationToken cancellationToken)
  {
    var existUpdatingNetworkDevice = await _networkDeviceRepository.ExistsAsync(x => x.Id == request.Id,
                                                                                cancellationToken);

    if (!existUpdatingNetworkDevice)
      throw new NetworkDeviceNotFoundException(request.Id.ToString());

    var updateNetworkDevice = request.Adapt<NetworkDevice>();

    await _networkDeviceRepository.ReplaceOneAsync(x => x.Id == updateNetworkDevice.Id,
                                                   updateNetworkDevice,
                                                   cancellationToken);
    return Unit.Value;
  }
}
