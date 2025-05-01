namespace Remote.Shell.Interrupt.Storehouse.Application.Features.TfPlans.Queries.GetTfPlansByFilter;

public record GetTfPlansByFilterQuery(RequestParameters RequestParameters) 
    : IQuery<PagedList<TfPlanDTO>>;

internal class GetTfPlansByFilterQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                              ITfPlanSpecification specification,
                                              IQueryFilterParser queryFilterParser,
                                              IMapper mapper) 
    : IQueryHandler<GetTfPlansByFilterQuery, PagedList<TfPlanDTO>>
{
  async Task<PagedList<TfPlanDTO>> IRequestHandler<GetTfPlansByFilterQuery, PagedList<TfPlanDTO>>.Handle(GetTfPlansByFilterQuery request,
                                                                                                         CancellationToken cancellationToken)
  {
    // Parse filter
    var filterExpr = queryFilterParser.ParseFilters<TfPlan>(request.RequestParameters
                                                                   .Filters);

    // Build base specification
    var baseSpec = BuildSpecification(specification,
                                      filterExpr);

    // Count records (without pagination)
    var countSpec = (ITfPlanSpecification)baseSpec.Clone();
    var count = await locBillUnitOfWork.TfPlans
                                       .GetCountAsync(countSpec,
                                                      cancellationToken);

    // Pagination parameters
    var pageNumber = request.RequestParameters.PageNumber ?? 0;
    var pageSize = request.RequestParameters.PageSize ?? 0;

    if (request.RequestParameters.EnablePagination)
        baseSpec.WithPagination(pageNumber,
                                pageSize);

    var tfPlans = await locBillUnitOfWork.TfPlans
                                         .GetManyShortAsync(baseSpec,
                                                            cancellationToken);

    if (!tfPlans.Any())
        return new PagedList<TfPlanDTO>([],0,0,0);

    var result = mapper.Map<IEnumerable<TfPlanDTO>>(tfPlans);

    // Return results
    return new PagedList<TfPlanDTO>(result,
                                    count,
                                    pageNumber,
                                    pageSize);
  }

  static ITfPlanSpecification BuildSpecification(ITfPlanSpecification baseSpec,
                                                 Expression<Func<TfPlan, bool>>? filterExpr)
  {
    var spec = baseSpec;

    if (filterExpr is not null)
        spec.AddFilter(filterExpr);

    return spec;
  }
}
