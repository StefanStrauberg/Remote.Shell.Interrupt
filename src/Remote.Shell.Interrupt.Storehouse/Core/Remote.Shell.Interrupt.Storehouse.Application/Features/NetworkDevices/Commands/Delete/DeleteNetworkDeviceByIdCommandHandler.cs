namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.Delete;

public record DeleteNetworkDeviceByIdCommand(Guid Id)
  : ICommand;

internal class DeleteNetworkDeviceByIdCommandHandler(IUnitOfWork unitOfWork)
  : ICommandHandler<DeleteNetworkDeviceByIdCommand, Unit>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));

  async Task<Unit> IRequestHandler<DeleteNetworkDeviceByIdCommand, Unit>.Handle(DeleteNetworkDeviceByIdCommand request,
                                                                                CancellationToken cancellationToken)
  {
    // Проверяем, существует ли устройство, соответствующее выражению фильтра
    var existingDeletingNetworkDevice = await _unitOfWork.NetworkDevices
                                                         .AnyByIdAsync(request.Id,
                                                                       cancellationToken);

    // Если устройство не найдено, выбрасываем исключение
    if (!existingDeletingNetworkDevice)
      throw new EntityNotFoundById(typeof(NetworkDevice),
                                   request.Id.ToString());

    // Получаем устройство для удаления
    var networkDeviceToDelete = await _unitOfWork.NetworkDevices
                                                 .FirstByIdAsync(request.Id,
                                                                 cancellationToken);

    // Удаляем найденное устройство из репозитория
    _unitOfWork.NetworkDevices
               .DeleteOne(networkDeviceToDelete);

    _unitOfWork.Complete();

    // Возвращаем успешный результат выполнения команды
    return Unit.Value;
  }
}
