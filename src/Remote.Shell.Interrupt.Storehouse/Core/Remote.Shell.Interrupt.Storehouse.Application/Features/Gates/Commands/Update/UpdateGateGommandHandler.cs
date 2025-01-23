namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.Update;

public record UpdateGateCommand(UpdateGateDTO UpdateGateDTO) : ICommand<Unit>;

internal class UpdateGateGommandHandler(IUnitOfWork unitOfWork,
                                        IMapper mapper)
  : ICommandHandler<UpdateGateCommand, Unit>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<Unit> IRequestHandler<UpdateGateCommand, Unit>.Handle(UpdateGateCommand request,
                                                                   CancellationToken cancellationToken)
  {
    // Проверка существует ли маршрутизатор
    var existingGate = await _unitOfWork.GateRepository
                                        .AnyByIdAsync(request.UpdateGateDTO.Id,
                                                      cancellationToken);

    // Если маршрутизатор не найдено, выбрасываем исключение
    if (!existingGate)
      throw new EntityNotFoundById(typeof(Gate),
                                   request.UpdateGateDTO.Id.ToString());

    // Находим маршрутизатор
    var gate = await _unitOfWork.GateRepository
                                .FirstByIdAsync(request.UpdateGateDTO.Id,
                                                cancellationToken);

    _mapper.Map(request.UpdateGateDTO, gate);

    _unitOfWork.GateRepository.ReplaceOne(gate);
    _unitOfWork.Complete();

    return Unit.Value;
  }
}
