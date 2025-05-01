namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.DeleteNetworkDeviceById;

public record DeleteNetworkDeviceByIdCommand(Guid Id)
  : ICommand;

internal class DeleteNetworkDeviceByIdCommandHandler(INetDevUnitOfWork netDevUnitOfWork,
                                                     INetworkDeviceSpecification specification,
                                                     IQueryFilterParser queryFilterParser)
  : ICommandHandler<DeleteNetworkDeviceByIdCommand, Unit>
{
  async Task<Unit> IRequestHandler<DeleteNetworkDeviceByIdCommand, Unit>.Handle(DeleteNetworkDeviceByIdCommand request,
                                                                                CancellationToken cancellationToken)
  {
    var requestParameters = new RequestParameters
    {
      Filters = [
        new ()
        {
          PropertyPath = "Id",
          Operator = FilterOperator.Equals,
          Value = request.Id.ToString()
        }
      ]
    };

    // Parse filter
    var filterExpr = queryFilterParser.ParseFilters<NetworkDevice>(requestParameters.Filters);

    // Build base specification
    var baseSpec = BuildSpecification(specification,
                                      filterExpr);

    // Проверка существования сетевого устройства с ID
    var existing = await netDevUnitOfWork.NetworkDevices
                                         .AnyByQueryAsync(baseSpec,
                                                          cancellationToken);
    
    // Если сетевоe устройства не найден — исключение
    if (!existing)
      throw new EntityNotFoundException(typeof(Gate),
                                        filterExpr is not null ? filterExpr.ToString() : string.Empty);

    // Получаем устройство для удаления
    var networkDeviceToDelete = await netDevUnitOfWork.NetworkDevices
                                                      .GetOneWithChildrenAsync(baseSpec,
                                                                                cancellationToken);

    // Удаляем найденное устройство из репозитория
    netDevUnitOfWork.NetworkDevices
                    .DeleteOneWithChilren(networkDeviceToDelete);

    netDevUnitOfWork.Complete();

    // Возвращаем успешный результат выполнения команды
    return Unit.Value;
  }

  static INetworkDeviceSpecification BuildSpecification(INetworkDeviceSpecification baseSpec,
                                               Expression<Func<NetworkDevice, bool>>? filterExpr)
  {
    var spec = baseSpec;

    if (filterExpr is not null)
        spec.AddFilter(filterExpr);

    return spec;
  }
}
