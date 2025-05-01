namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientWithChildrenByFilters;

public record GetClientWithChildrenByFiltersQuery(RequestParametersUpdated RequestParameters) : IRequest<DetailClientDTO>;

internal class GetClientWithChildrenByFiltersHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                                     IClientSpecification clientSpecification,
                                                     IQueryFilterParser queryFilterParser,
                                                     IMapper mapper) 
    : IRequestHandler<GetClientWithChildrenByFiltersQuery, DetailClientDTO>
{

  async Task<DetailClientDTO> IRequestHandler<GetClientWithChildrenByFiltersQuery, DetailClientDTO>.Handle(GetClientWithChildrenByFiltersQuery request,
                                                                                                           CancellationToken cancellationToken)
  {
    // Parse filter
    var filterExpr = queryFilterParser.ParseFilters<Client>(request.RequestParameters
                                                                   .Filters);

    // Build base specification
    var baseSpec = BuildSpecification(clientSpecification,
                                      filterExpr);

    // Проверка существования клиент с ID
    var existingClient = await locBillUnitOfWork.Clients
                                                .AnyByQueryAsync(baseSpec,
                                                                 cancellationToken);

    // Если клиент не найдено — исключение
    if (!existingClient)
      throw new EntityNotFoundException(typeof(Client),
                                        filterExpr is not null ? filterExpr.ToString() : string.Empty);

    var client = await locBillUnitOfWork.Clients
                                        .GetOneWithChildrensAsync(baseSpec,
                                                                  cancellationToken);
    
    var result = mapper.Map<DetailClientDTO>(client);
    
    return result;
  }

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