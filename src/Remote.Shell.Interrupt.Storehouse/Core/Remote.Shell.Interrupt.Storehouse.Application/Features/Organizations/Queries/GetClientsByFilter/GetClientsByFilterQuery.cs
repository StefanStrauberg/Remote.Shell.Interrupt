namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientsByFilter;

public record GetClientsByFilterQuery(RequestParametersUpdated RequestParameters) 
  : IQuery<PagedList<ShortClientDTO>>;

internal class GetClientsByFilterQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                              IClientSpecification clientSpecification,
                                              IQueryFilterParser queryFilterParser,
                                              IMapper mapper)
  : IQueryHandler<GetClientsByFilterQuery, PagedList<ShortClientDTO>>
{
  async Task<PagedList<ShortClientDTO>> IRequestHandler<GetClientsByFilterQuery, PagedList<ShortClientDTO>>.Handle(GetClientsByFilterQuery request,
                                                                                                                   CancellationToken cancellationToken)
  {
    // Parse filter
    var filterExpr = queryFilterParser.ParseFilters<Client>(request.RequestParameters
                                                                   .Filters);

    // Build base specification
    var baseSpec = BuildSpecification(clientSpecification,
                                      filterExpr);

    // Count records (without pagination)
    var countSpec = (IClientSpecification)baseSpec.Clone();
    var count = await locBillUnitOfWork.Clients
                                       .GetCountAsync(countSpec,
                                                      cancellationToken);

    // Pagination parameters
    var pageNumber = request.RequestParameters.PageNumber ?? 0;
    var pageSize = request.RequestParameters.PageSize ?? 0;

    if (request.RequestParameters.EnablePagination)
        baseSpec.WithPagination(pageNumber,
                                pageSize);

    var clients = await locBillUnitOfWork.Clients
                                         .GetManyShortAsync(baseSpec,
                                                            cancellationToken);

    if (!clients.Any())
      return new PagedList<ShortClientDTO>([],0,0,0);

    var result = mapper.Map<IEnumerable<ShortClientDTO>>(clients);

    // Return results
    return new PagedList<ShortClientDTO>(result,
                                         count,
                                         pageNumber,
                                         pageSize);
  }

  static IClientSpecification BuildSpecification(IClientSpecification baseSpec,
                                                 Expression<Func<Client, bool>>? filterExpr)
  {
    var spec = baseSpec;

    if (filterExpr is not null)
        spec.AddFilter(filterExpr);

    return spec;
  }
}
