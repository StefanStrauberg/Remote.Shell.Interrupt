namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.CreateGate;

public record CreateGateCommand(CreateGateDTO CreateGateDTO) : ICommand<Unit>;

internal class CreateGateCommandHandler(IGateUnitOfWork gateUnitOfWork,
                                        IGateSpecification specification,
                                        IQueryFilterParser queryFilterParser,
                                        IMapper mapper)
  : ICommandHandler<CreateGateCommand, Unit>
{
  public async Task<Unit> Handle(CreateGateCommand request,
                                 CancellationToken cancellationToken)
  {
    var requestParameters = new RequestParameters
    {
      Filters = [
        new ()
        {
          PropertyPath = "IPAddress",
          Operator = FilterOperator.Equals,
          Value = request.CreateGateDTO.IPAddress
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
    
    // Если шлюз найден — исключение
    if (existing)
      throw new EntityAlreadyExists(typeof(Gate),
                                    filterExpr is not null ? filterExpr.ToString() : string.Empty);

    var gate = mapper.Map<Gate>(request.CreateGateDTO);

    gateUnitOfWork.Gates
                  .InsertOne(gate);

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
