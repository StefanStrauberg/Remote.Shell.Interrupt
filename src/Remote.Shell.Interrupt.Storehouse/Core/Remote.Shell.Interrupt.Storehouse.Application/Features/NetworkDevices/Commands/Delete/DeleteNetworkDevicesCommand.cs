namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.Delete;

public record DeleteNetworkDevicesCommand : ICommand<Unit>;

internal class DeleteNetworkDevicesCommandHandler(INetDevUnitOfWork netDevUnitOfWork)
  : ICommandHandler<DeleteNetworkDevicesCommand, Unit>
{
  async Task<Unit> IRequestHandler<DeleteNetworkDevicesCommand, Unit>.Handle(DeleteNetworkDevicesCommand request,
                                                                             CancellationToken cancellationToken)
  {
    var networkDevices = await netDevUnitOfWork.NetworkDevices
                                               .GetAllAsync(cancellationToken);

    var deleteNetworkDeviceByIdCommandHandler = new DeleteNetworkDeviceByIdCommandHandler(netDevUnitOfWork);

    foreach (var item in networkDevices)
    {
      var deleteNetworkDeviceByIdCommand = new DeleteNetworkDeviceByIdCommand(item.Id);

      await ((IRequestHandler<DeleteNetworkDeviceByIdCommand, Unit>)deleteNetworkDeviceByIdCommandHandler).Handle(deleteNetworkDeviceByIdCommand,
                                                                                                                  cancellationToken);
    }

    // Возвращаем успешный результат выполнения команды
    return Unit.Value;
  }
}
