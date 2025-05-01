namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Queries.GetGateById;

public record GetGateByIdQuery(Guid Id) : IQuery<GateDTO>;

internal class GetGateByIdQueryHandler(IGateUnitOfWork gateUnitOfWork,
                                       IGateSpecification specification,
                                       IQueryFilterParser queryFilterParser,
                                       IMapper mapper)
  : IQueryHandler<GetGateByIdQuery, GateDTO>
{
  async Task<GateDTO> IRequestHandler<GetGateByIdQuery, GateDTO>.Handle(GetGateByIdQuery request,
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

    // Находим шлюз
    var gate = await gateUnitOfWork.Gates
                                   .GetOneShortAsync(baseSpec,
                                                     cancellationToken);

    var result = mapper.Map<GateDTO>(gate);

    return result;
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
