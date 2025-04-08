namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.Delete;

public record DeleteNetworkDevicesCommand : ICommand<Unit>;

internal class DeleteNetworkDevicesCommandHandler(IUnitOfWork unitOfWork)
  : ICommandHandler<DeleteNetworkDevicesCommand, Unit>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));

  async Task<Unit> IRequestHandler<DeleteNetworkDevicesCommand, Unit>.Handle(DeleteNetworkDevicesCommand request,
                                                                             CancellationToken cancellationToken)
  {
    var networkDevices = await _unitOfWork.NetworkDevices
                                          .GetAllAsync(cancellationToken);

    foreach (var networkDevice in networkDevices)
    {
      // Удаляем найденное устройство из репозитория
      _unitOfWork.NetworkDevices
                 .DeleteOneWithChilren(networkDevice);
    }

    _unitOfWork.Complete();

    // Возвращаем успешный результат выполнения команды
    return Unit.Value;
  }
}
