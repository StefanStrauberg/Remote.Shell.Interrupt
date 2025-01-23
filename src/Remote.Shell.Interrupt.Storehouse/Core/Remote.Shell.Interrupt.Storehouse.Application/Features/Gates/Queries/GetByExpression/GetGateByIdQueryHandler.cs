namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Queries.GetByExpression;

public record GetGateByIdQuery(Guid Id) : IQuery<GateDTO>;

internal class GetGateByIdQueryHandler(IUnitOfWork unitOfWork,
                                       IMapper mapper)
  : IQueryHandler<GetGateByIdQuery, GateDTO>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<GateDTO> IRequestHandler<GetGateByIdQuery, GateDTO>.Handle(GetGateByIdQuery request,
                                                                        CancellationToken cancellationToken)
  {
    // Проверка существует ли маршрутизатор
    var existingGate = await _unitOfWork.GateRepository
                                        .AnyByIdAsync(request.Id,
                                                      cancellationToken);

    // Если маршрутизатор не найдено, выбрасываем исключение
    if (!existingGate)
      throw new EntityNotFoundById(typeof(Gate),
                                   request.Id.ToString());

    // Находим маршрутизатор
    var gate = await _unitOfWork.GateRepository
                                .FirstByIdAsync(request.Id,
                                                cancellationToken);

    var result = _mapper.Map<GateDTO>(gate);

    return result;
  }
}
