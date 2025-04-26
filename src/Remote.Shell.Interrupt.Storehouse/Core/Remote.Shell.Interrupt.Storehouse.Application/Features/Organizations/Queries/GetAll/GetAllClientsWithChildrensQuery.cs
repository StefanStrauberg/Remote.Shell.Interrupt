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
    var pageNumber = request.RequestParameters.PageNumber;
    var pageSize = request.RequestParameters.PageSize;

    // Парсим фильтр
    var filterExpression = queryFilterParser.ParseFilters<Client>(request.RequestParameters
                                                                         .Filters);

    var spec = clientSpecification.AddFilter(filterExpression!)
                                  .AddInclude(c => c.COD)
                                  .AddInclude(c => c.TfPlanL!)
                                  .AddInclude(c => c.SPRVlans)
                                  .WithPagination(pageNumber,
                                                  pageSize); // Добавляем пагинацию;

    var clients = await locBillUnitOfWork.Clients
                                         .GetManyWithChildrenAsync(spec,
                                                                   cancellationToken);

    var count = await locBillUnitOfWork.Clients
                                       .GetCountAsync(spec,
                                                      cancellationToken);

    var result = mapper.Map<IEnumerable<DetailClientDTO>>(clients);

    return new PagedList<DetailClientDTO>(result,
                                          count,
                                          pageNumber,
                                          pageSize);
  }
}
