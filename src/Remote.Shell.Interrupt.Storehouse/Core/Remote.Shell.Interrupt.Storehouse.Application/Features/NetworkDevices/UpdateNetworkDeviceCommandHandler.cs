namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices;

public record UpdateNetworkDeviceCommand(Guid Id,
                                         IPAddress Host,
                                         string Vendor,
                                         string Model,
                                         string SoftwareVersion,
                                         GatewayLevel GatewayLevel,
                                         ICollection<Interface> Interfaces) : ICommand;

internal class UpdateNetworkDeviceCommandHandler(INetworkDeviceRepository networkDeviceRepository)
  : ICommandHandler<UpdateNetworkDeviceCommand, Unit>
{
  readonly INetworkDeviceRepository _networkDeviceRepository = networkDeviceRepository
    ?? throw new ArgumentNullException(nameof(networkDeviceRepository));

  async Task<Unit> IRequestHandler<UpdateNetworkDeviceCommand, Unit>.Handle(UpdateNetworkDeviceCommand request,
                                                                            CancellationToken cancellationToken)
  {
    var existingUpdatingNetworkDevice = await _networkDeviceRepository.ExistsAsync(filterExpression: x => x.Id == request.Id,
                                                                                   cancellationToken: cancellationToken);

    if (!existingUpdatingNetworkDevice)
      throw new EntityNotFoundException(request.Id.ToString());

    var updateNetworkDevice = request.Adapt<NetworkDevice>();

    await _networkDeviceRepository.ReplaceOneAsync(filterExpression: x => x.Id == request.Id,
                                                   document: updateNetworkDevice,
                                                   cancellationToken: cancellationToken);
    return Unit.Value;
  }
}
