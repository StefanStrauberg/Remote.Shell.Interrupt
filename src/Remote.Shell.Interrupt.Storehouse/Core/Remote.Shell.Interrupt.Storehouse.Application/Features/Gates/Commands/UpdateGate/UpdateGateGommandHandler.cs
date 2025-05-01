namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.UpdateGate;

public record UpdateGateCommand(UpdateGateDTO UpdateGateDTO) : ICommand<Unit>;

internal class UpdateGateGommandHandler(IGateUnitOfWork gateUnitOfWork,
                                        IGateSpecification specification,
                                        IQueryFilterParser queryFilterParser,
                                        IMapper mapper)
  : ICommandHandler<UpdateGateCommand, Unit>
{

  async Task<Unit> IRequestHandler<UpdateGateCommand, Unit>.Handle(UpdateGateCommand request,
                                                                   CancellationToken cancellationToken)
  {
    var requestParameters = new RequestParameters
    {
      Filters = [
        new ()
        {
          PropertyPath = "Id",
          Operator = FilterOperator.Equals,
          Value = request.UpdateGateDTO.Id.ToString()
        }
      ]
    };

    // Parse filter
    var filterExpr = queryFilterParser.ParseFilters<Gate>(requestParameters.Filters);

    // Build base specification
    var baseSpec = BuildSpecification(specification,
                                      filterExpr);

    // Проверка существования шлюза с ID
    var existing = await gateUnitOfWork.Gates
                                       .AnyByQueryAsync(baseSpec,
                                                        cancellationToken);
    
    // Если шлюз не найден — исключение
    if (!existing)
      throw new EntityNotFoundException(typeof(Gate),
                                        filterExpr is not null ? filterExpr.ToString() : string.Empty);

    // Находим маршрутизатор
    var gate = await gateUnitOfWork.Gates
                                   .GetOneShortAsync(baseSpec,
                                                     cancellationToken);

    mapper.Map(request.UpdateGateDTO, gate);

    gateUnitOfWork.Gates
                  .ReplaceOne(gate);

    gateUnitOfWork.Complete();

    return Unit.Value;
  }

  static IGateSpecification BuildSpecification(IGateSpecification baseSpec,
                                               Expression<Func<Gate, bool>>? filterExpr)
  {
    var spec = baseSpec;

    if (filterExpr is not null)
        spec.AddFilter(filterExpr);

    return spec;
  }
}
