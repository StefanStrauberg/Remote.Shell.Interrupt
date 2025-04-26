namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetAll;

public record GetAllClientsWithChildrensQuery(RequestParametersUpdated RequestParameters) 
  : IQuery<PagedList<DetailClientDTO>>;

internal class GetAllClientsWithChildrensQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                                      IClientSpecification clientSpecification,
                                                      IQueryFilterParser queryFilterParser,
                                                      IMapper mapper)
  : IQueryHandler<GetAllClientsWithChildrensQuery, PagedList<DetailClientDTO>>
{
  async Task<PagedList<DetailClientDTO>> IRequestHandler<GetAllClientsWithChildrensQuery, PagedList<DetailClientDTO>>.Handle(GetAllClientsWithChildrensQuery request,
                                                                                                                             CancellationToken cancellationToken)
  {
    // Парсим фильтр
    var filterExpression = queryFilterParser.ParseFilters<Client>(request.RequestParameters
                                                                         .Filters);

    var spec = clientSpecification.AddFilter(filterExpression!)
                                  .AddInclude(c => c.COD)
                                  .AddInclude(c => c.TfPlanL!);

    var clients = await locBillUnitOfWork.Clients
                                         .GetManyWithChildrenAsync(spec,
                                                                   cancellationToken);
                                                                        
    if (!clients.Any())
      return new PagedList<DetailClientDTO>([],0,0,0);

    var count = await locBillUnitOfWork.Clients
                                       .GetCountAsync(spec,
                                                      cancellationToken);

    var result = mapper.Map<IEnumerable<DetailClientDTO>>(clients);

    return new PagedList<DetailClientDTO>(result,
                                          count,
                                          request.RequestParameters.PageNumber,
                                          request.RequestParameters.PageSize);
  }
}
