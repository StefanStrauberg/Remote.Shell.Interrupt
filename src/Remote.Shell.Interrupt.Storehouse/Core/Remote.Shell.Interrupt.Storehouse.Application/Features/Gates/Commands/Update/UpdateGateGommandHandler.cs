namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.Update;

public record UpdateGateCommand(UpdateGateDTO UpdateGateDTO) : ICommand<Unit>;

internal class UpdateGateGommandHandler(IGateUnitOfWork gateUnitOfWork,
                                        IMapper mapper)
  : ICommandHandler<UpdateGateCommand, Unit>
{

  async Task<Unit> IRequestHandler<UpdateGateCommand, Unit>.Handle(UpdateGateCommand request,
                                                                   CancellationToken cancellationToken)
  {
    var requestParameters = new RequestParameters
    {
      Filters = $"Id=={request.UpdateGateDTO.Id}"
    };

    // Проверка существует ли маршрутизатор
    var existingGate = await gateUnitOfWork.GateRepository
                                           .AnyByQueryAsync(requestParameters,
                                                            cancellationToken);

    // Если маршрутизатор не найдено, выбрасываем исключение
    if (!existingGate)
      throw new EntityNotFoundById(typeof(Gate),
                                   request.UpdateGateDTO.Id.ToString());

    // Находим маршрутизатор
    var gate = await gateUnitOfWork.GateRepository
                                   .GetOneShortAsync(requestParameters,
                                                     cancellationToken);

    mapper.Map(request.UpdateGateDTO, gate);

    gateUnitOfWork.GateRepository.ReplaceOne(gate);
    gateUnitOfWork.Complete();

    return Unit.Value;
  }
}
