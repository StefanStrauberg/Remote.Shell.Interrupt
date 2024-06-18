
namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices;

public record DeleteNetworkDeviceCommand(Guid Id) : ICommand;

internal class DeleteNetworkDeviceCommandHandler(INetworkDeviceRepository networkDeviceRepository)
  : ICommandHandler<DeleteNetworkDeviceCommand, Unit>
{
  readonly INetworkDeviceRepository _networkDeviceRepository = networkDeviceRepository
    ?? throw new ArgumentNullException(nameof(networkDeviceRepository));
  async Task<Unit> IRequestHandler<DeleteNetworkDeviceCommand, Unit>.Handle(DeleteNetworkDeviceCommand request,
                                                                            CancellationToken cancellationToken)
  {
    var existDeletingNetworkDevice = await _networkDeviceRepository.ExistsAsync(x => x.Id == request.Id,
                                                                                cancellationToken);

    if (!existDeletingNetworkDevice)
      throw new NetworkDeviceNotFoundException(request.Id.ToString());

    await _networkDeviceRepository.DeleteOneAsync(x => x.Id == request.Id,
                                                  cancellationToken);
    return Unit.Value;
  }
}
