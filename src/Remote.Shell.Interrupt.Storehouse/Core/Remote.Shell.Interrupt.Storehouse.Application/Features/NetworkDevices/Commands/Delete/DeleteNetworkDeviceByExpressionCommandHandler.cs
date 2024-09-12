namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.Delete;

internal class DeleteNetworkDeviceByExpressionCommandHandler(INetworkDeviceRepository networkDeviceRepository)
  : ICommandHandler<DeleteNetworkDeviceByExpressionCommand, Unit>
{
  readonly INetworkDeviceRepository _networkDeviceRepository = networkDeviceRepository
    ?? throw new ArgumentNullException(nameof(networkDeviceRepository));

  async Task<Unit> IRequestHandler<DeleteNetworkDeviceByExpressionCommand, Unit>.Handle(DeleteNetworkDeviceByExpressionCommand request,
                                                                                        CancellationToken cancellationToken)
  {
    // Проверяем, существует ли устройство, соответствующее выражению фильтра
    var existingDeletingNetworkDevice = await _networkDeviceRepository.ExistsAsync(filterExpression: request.FilterExpression,
                                                                                   cancellationToken: cancellationToken);

    // Если устройство не найдено, выбрасываем исключение
    if (!existingDeletingNetworkDevice)
      throw new EntityNotFoundException(request.ToString());

    // Находим устройство, которое нужно удалить, по выражению фильтра
    var networkDeviceToDelete = await _networkDeviceRepository.FindOneAsync(filterExpression: request.FilterExpression,
                                                                            cancellationToken: cancellationToken);

    // Удаляем найденное устройство из репозитория
    await _networkDeviceRepository.DeleteOneAsync(document: networkDeviceToDelete,
                                                  cancellationToken: cancellationToken);

    // Возвращаем успешный результат выполнения команды
    return Unit.Value;
  }
}
