namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetAll;

public record GetAllClientsWithChildrensQuery(UpdatedRequestParameters RequestParameters) 
  : IQuery<PagedList<DetailClientDTO>>;

internal class GetAllClientsWithChildrensQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                                      IMapper mapper)
  : IQueryHandler<GetAllClientsWithChildrensQuery, PagedList<DetailClientDTO>>
{
  async Task<PagedList<DetailClientDTO>> IRequestHandler<GetAllClientsWithChildrensQuery, PagedList<DetailClientDTO>>.Handle(GetAllClientsWithChildrensQuery request,
                                                                                                                             CancellationToken cancellationToken)
  {
    // Парсим фильтр
    var filterExpression = FilterParser.ParseFilters<Client>(request.RequestParameters
                                                                    .Filters);

    var spec = new ClientSpecification().AddFilter(filterExpression!)
                                        .AddInclude(c => c.COD)
                                        .AddInclude(c => c.TfPlanL!);

    // Проверка: добавляем пагинацию, только если параметры указаны
    if (request.RequestParameters.PageNumber > 0 && request.RequestParameters.PageSize > 0)
        spec.WithPagination(request.RequestParameters.PageNumber,
                            request.RequestParameters.PageSize);

    var clients = await locBillUnitOfWork.Clients
                                         .GetManyWithChildrenAsync(spec,
                                                                   cancellationToken);
                                                                        
    if (!clients.Any())
      return default!;

    // TODO - Spec
    // var count = await locBillUnitOfWork.Clients
    //                                    .GetCountAsync(request.RequestParameters,
    //                                                   cancellationToken);

    var result = mapper.Map<IEnumerable<DetailClientDTO>>(clients);

    return new PagedList<DetailClientDTO>(result,
                                          0 > 0 ? result.Count() 
                                                : 0,
                                          request.RequestParameters.PageNumber,
                                          request.RequestParameters.PageSize > 0 ? request.RequestParameters.PageSize 
                                                                                 : result.Count());
  }
}
