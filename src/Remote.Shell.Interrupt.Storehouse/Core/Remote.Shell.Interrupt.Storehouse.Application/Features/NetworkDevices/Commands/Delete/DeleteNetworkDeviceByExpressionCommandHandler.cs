namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.Delete;

internal class DeleteNetworkDeviceByExpressionCommandHandler(INetworkDeviceRepository networkDeviceRepository)
  : ICommandHandler<DeleteNetworkDeviceByExpressionCommand, Unit>
{
  readonly INetworkDeviceRepository _networkDeviceRepository = networkDeviceRepository
    ?? throw new ArgumentNullException(nameof(networkDeviceRepository));

  async Task<Unit> IRequestHandler<DeleteNetworkDeviceByExpressionCommand, Unit>.Handle(DeleteNetworkDeviceByExpressionCommand request,
                                                                                        CancellationToken cancellationToken)
  {
    var existingDeletingNetworkDevice = await _networkDeviceRepository.ExistsAsync(filterExpression: request.FilterExpression,
                                                                                   cancellationToken: cancellationToken);

    if (!existingDeletingNetworkDevice)
      throw new EntityNotFoundException(request.ToString());

    await _networkDeviceRepository.DeleteOneAsync(filterExpression: request.FilterExpression,
                                                  cancellationToken: cancellationToken);
    return Unit.Value;
  }
}
