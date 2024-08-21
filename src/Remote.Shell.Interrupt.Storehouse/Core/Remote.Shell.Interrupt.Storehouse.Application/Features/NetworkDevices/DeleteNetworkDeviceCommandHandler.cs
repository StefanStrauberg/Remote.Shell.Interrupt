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
    var existingDeletingNetworkDevice = await _networkDeviceRepository.ExistsAsync(filterExpression: x => x.Id == request.Id,
                                                                                   cancellationToken: cancellationToken);

    if (!existingDeletingNetworkDevice)
      throw new EntityNotFoundException(request.Id.ToString());

    await _networkDeviceRepository.DeleteOneAsync(filterExpression: x => x.Id == request.Id,
                                                  cancellationToken: cancellationToken);
    return Unit.Value;
  }
}
