namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.Delete;

public record DeleteNetworkDeviceByIdCommand(Guid Id)
  : ICommand;

internal class DeleteNetworkDeviceByIdCommandHandler(INetDevUnitOfWork netDevUnitOfWork)
  : ICommandHandler<DeleteNetworkDeviceByIdCommand, Unit>
{
  async Task<Unit> IRequestHandler<DeleteNetworkDeviceByIdCommand, Unit>.Handle(DeleteNetworkDeviceByIdCommand request,
                                                                                CancellationToken cancellationToken)
  {
    var requestParameters = new RequestParameters
    {
      Filters = $"Id=={request.Id}"
    };

    // Проверяем, существует ли устройство, соответствующее выражению фильтра
    var existingDeletingNetworkDevice = await netDevUnitOfWork.NetworkDevices
                                                              .AnyByQueryAsync(requestParameters,
                                                                               cancellationToken);

    // Если устройство не найдено, выбрасываем исключение
    if (!existingDeletingNetworkDevice)
      throw new EntityNotFoundById(typeof(NetworkDevice),
                                   request.Id.ToString());

    // Получаем устройство для удаления
    var networkDeviceToDelete = await netDevUnitOfWork.NetworkDevices
                                                      .GetOneWithChildrensAsync(requestParameters,
                                                                                cancellationToken);

    // Удаляем найденное устройство из репозитория
    netDevUnitOfWork.NetworkDevices
                    .DeleteOneWithChilren(networkDeviceToDelete);

    netDevUnitOfWork.Complete();

    // Возвращаем успешный результат выполнения команды
    return Unit.Value;
  }
}
