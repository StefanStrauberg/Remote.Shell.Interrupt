namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientById;

public record GetClientByIdQuery(Guid Id) : IQuery<DetailClientDTO>;

internal class GetClientByIdQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                         IClientSpecification specification,
                                         IQueryFilterParser queryFilterParser,
                                         IMapper mapper) 
  : IQueryHandler<GetClientByIdQuery, DetailClientDTO>
{
  async Task<DetailClientDTO> IRequestHandler<GetClientByIdQuery, DetailClientDTO>.Handle(GetClientByIdQuery request,
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
    var filterExpr = queryFilterParser.ParseFilters<Client>(requestParameters.Filters);

    // Build base specification
    var baseSpec = BuildSpecification(specification,
                                      filterExpr);

    // Проверка существования клиент с ID
    var existing = await locBillUnitOfWork.Clients
                                          .AnyByQueryAsync(baseSpec,
                                                           cancellationToken);

    // Если клиент не найден — исключение
    if (!existing)
      throw new EntityNotFoundException(typeof(Client),
                                        filterExpr is not null ? filterExpr.ToString() : string.Empty);

    // Находим клиента
    var client = await locBillUnitOfWork.Clients
                                        .GetOneWithChildrenAsync(baseSpec,
                                                                  cancellationToken);

    var result = mapper.Map<DetailClientDTO>(client);

    // Return result
    return result;
  }

  /// <summary>
  /// Builds the specification with included entities and filters.
  /// </summary>
  /// <param name="baseSpec">The base client specification.</param>
  /// <param name="filterExpr">The filter expression to apply.</param>
  /// <returns>
  /// An updated client specification including related entities and filters.
  /// </returns>
  static IClientSpecification BuildSpecification(IClientSpecification baseSpec,
                                                 Expression<Func<Client, bool>>? filterExpr)
  {
    var spec = baseSpec.AddInclude(c => c.COD)
                       .AddInclude(c => c.TfPlan!)
                       .AddInclude(c => c.SPRVlans);

    if (filterExpr is not null)
        spec.AddFilter(filterExpr);

    return (IClientSpecification)spec;
  }
}