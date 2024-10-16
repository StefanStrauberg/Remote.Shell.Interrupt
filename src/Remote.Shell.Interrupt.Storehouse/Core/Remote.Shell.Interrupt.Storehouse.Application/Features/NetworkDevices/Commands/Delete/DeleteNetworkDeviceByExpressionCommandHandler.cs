namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.Delete;

internal class DeleteNetworkDeviceByExpressionCommandHandler(IUnitOfWork unitOfWork)
  : ICommandHandler<DeleteNetworkDeviceByExpressionCommand, Unit>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));

  async Task<Unit> IRequestHandler<DeleteNetworkDeviceByExpressionCommand, Unit>.Handle(DeleteNetworkDeviceByExpressionCommand request,
                                                                                        CancellationToken cancellationToken)
  {
    // Проверяем, существует ли устройство, соответствующее выражению фильтра
    var existingDeletingNetworkDevice = await _unitOfWork.NetworkDevices
                                                         .AnyAsync(request.FilterExpression, cancellationToken);

    // Если устройство не найдено, выбрасываем исключение
    if (!existingDeletingNetworkDevice)
      throw new EntityNotFoundException(request.ToString());

    // Находим устройство, которое нужно удалить, по выражению фильтра
    var networkDeviceToDelete = await _unitOfWork.NetworkDevices
                                                 .FirstAsync(request.FilterExpression, cancellationToken);

    // Удаляем найденное устройство из репозитория
    _unitOfWork.NetworkDevices
               .DeleteOne(networkDeviceToDelete);

    await _unitOfWork.CompleteAsync(cancellationToken);

    // Возвращаем успешный результат выполнения команды
    return Unit.Value;
  }
}
