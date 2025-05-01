namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Queries.GetGatesByFilter;

public record GetGatesByFilterQuery(RequestParameters RequestParameters) 
  : IQuery<PagedList<GateDTO>>;

internal class GetGatesByFilterQueryHandler(IGateUnitOfWork gateUnitOfWork,
                                            IGateSpecification specification,
                                            IQueryFilterParser queryFilterParser,
                                            IMapper mapper)
  : IQueryHandler<GetGatesByFilterQuery, PagedList<GateDTO>>
{
  async Task<PagedList<GateDTO>> IRequestHandler<GetGatesByFilterQuery, PagedList<GateDTO>>.Handle(GetGatesByFilterQuery request,
                                                                                           CancellationToken cancellationToken)
  {
    // Parse filter
    var filterExpr = queryFilterParser.ParseFilters<Gate>(request.RequestParameters
                                                                   .Filters);

    // Build base specification
    var baseSpec = BuildSpecification(specification,
                                      filterExpr);

    // Count records (without pagination)
    var countSpec = (IGateSpecification)baseSpec.Clone();
    var count = await gateUnitOfWork.Gates
                                    .GetCountAsync(countSpec,
                                                   cancellationToken);

    // Pagination parameters
    var pageNumber = request.RequestParameters.PageNumber ?? 0;
    var pageSize = request.RequestParameters.PageSize ?? 0;

    if (request.RequestParameters.EnablePagination)
        baseSpec.WithPagination(pageNumber,
                                pageSize);

    var gates = await gateUnitOfWork.Gates
                                    .GetManyShortAsync(baseSpec,
                                                       cancellationToken);
                                                   
    if (!gates.Any())
      return new PagedList<GateDTO>([],0,0,0);

    var result = mapper.Map<IEnumerable<GateDTO>>(gates);

    // Return results
    return new PagedList<GateDTO>(result,
                                  count,
                                  pageNumber,
                                  pageSize);
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
