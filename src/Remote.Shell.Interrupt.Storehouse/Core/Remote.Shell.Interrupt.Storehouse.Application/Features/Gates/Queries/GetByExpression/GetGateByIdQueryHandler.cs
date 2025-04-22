namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Queries.GetByExpression;

public record GetGateByIdQuery(Guid Id) : IQuery<GateDTO>;

internal class GetGateByIdQueryHandler(IGateUnitOfWork gateUnitOfWork,
                                       IMapper mapper)
  : IQueryHandler<GetGateByIdQuery, GateDTO>
{
  async Task<GateDTO> IRequestHandler<GetGateByIdQuery, GateDTO>.Handle(GetGateByIdQuery request,
                                                                        CancellationToken cancellationToken)
  {
    var requestParameters = new RequestParameters
    {
      Filters = $"Id=={request.Id}"
    };

    // Проверка существует ли маршрутизатор
    var existingGate = await gateUnitOfWork.GateRepository
                                           .AnyByQueryAsync(requestParameters,
                                                            cancellationToken);

    // Если маршрутизатор не найдено, выбрасываем исключение
    if (!existingGate)
      throw new EntityNotFoundById(typeof(Gate),
                                   request.Id.ToString());

    // Находим маршрутизатор
    var gate = await gateUnitOfWork.GateRepository
                                   .GetOneShortAsync(requestParameters,
                                                     cancellationToken);

    var result = mapper.Map<GateDTO>(gate);

    return result;
  }
}
