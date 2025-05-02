namespace Remote.Shell.Interrupt.Storehouse.Application.Features.TfPlans.Queries.GetTfPlansByFilter;

/// <summary>
/// Represents a query to retrieve TF plans based on filtering criteria.
/// </summary>
/// <param name="RequestParameters">The request parameters containing filtering and pagination settings.</param>
public record GetTfPlansByFilterQuery(RequestParameters RequestParameters) 
    : IQuery<PagedList<TfPlanDTO>>;

/// <summary>
/// Handles the GetTfPlansByFilterQuery and retrieves filtered TF plans.
/// </summary>
/// <remarks>
/// This handler applies filtering expressions, constructs the necessary specifications,
/// manages pagination, retrieves TF plans, and returns the mapped results.
/// </remarks>
/// <param name="locBillUnitOfWork">Unit of work for TF plan-related database operations.</param>
/// <param name="specification">Specification used for filtering TF plan entities.</param>
/// <param name="queryFilterParser">Parser for processing filter expressions.</param>
/// <param name="mapper">Object mapper for DTO transformation.</param>
internal class GetTfPlansByFilterQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                              ITfPlanSpecification specification,
                                              IQueryFilterParser queryFilterParser,
                                              IMapper mapper) 
    : IQueryHandler<GetTfPlansByFilterQuery, PagedList<TfPlanDTO>>
{
  /// <summary>
  /// Handles the request to retrieve TF plans based on filtering criteria.
  /// </summary>
  /// <param name="request">The query request containing filtering and pagination parameters.</param>
  /// <param name="cancellationToken">Token to support request cancellation.</param>
  /// <returns>A paginated list of TF plan DTOs.</returns>
  async Task<PagedList<TfPlanDTO>> IRequestHandler<GetTfPlansByFilterQuery, PagedList<TfPlanDTO>>.Handle(GetTfPlansByFilterQuery request,
                                                                                                         CancellationToken cancellationToken)
  {
    // Parse the filter expression
    var filterExpr = queryFilterParser.ParseFilters<TfPlan>(request.RequestParameters
                                                                   .Filters);

    // Build the base specification with filtering applied
    var baseSpec = BuildSpecification(specification,
                                      filterExpr);

    // Create a specification for counting total matching records
    var countSpec = baseSpec.Clone();

    // Extract pagination parameters
    var pageNumber = request.RequestParameters.PageNumber ?? 0;
    var pageSize = request.RequestParameters.PageSize ?? 0;

    // Apply pagination settings if enabled
    if (request.RequestParameters.EnablePagination)
        baseSpec.WithPagination(pageNumber,
                                pageSize);

    // Retrieve data, including related entities
    var tfPlans = await locBillUnitOfWork.TfPlans
                                         .GetManyShortAsync(baseSpec,
                                                            cancellationToken);

    // Return an empty paginated list if no data are found.
    if (!tfPlans.Any())
        return new PagedList<TfPlanDTO>([],0,0,0);

    // Retrieve the total count of matching records
    var count = await locBillUnitOfWork.TfPlans
                                       .GetCountAsync(countSpec,
                                                      cancellationToken);

    // Map the retrieved data to the DTO
    var result = mapper.Map<IEnumerable<TfPlanDTO>>(tfPlans);

    // Return the mapped result
    return new PagedList<TfPlanDTO>(result,
                                    count,
                                    pageNumber,
                                    pageSize);
  }

  /// <summary>
  /// Builds the specification by applying filtering criteria.
  /// </summary>
  /// <param name="baseSpec">The base TF plan specification.</param>
  /// <param name="filterExpr">The filter expression to apply.</param>
  /// <returns>An updated specification with filtering applied.</returns>
  static ITfPlanSpecification BuildSpecification(ITfPlanSpecification baseSpec,
                                                 Expression<Func<TfPlan, bool>>? filterExpr)
  {
    var spec = baseSpec;

    if (filterExpr is not null)
        spec.AddFilter(filterExpr);

    return spec;
  }
}
