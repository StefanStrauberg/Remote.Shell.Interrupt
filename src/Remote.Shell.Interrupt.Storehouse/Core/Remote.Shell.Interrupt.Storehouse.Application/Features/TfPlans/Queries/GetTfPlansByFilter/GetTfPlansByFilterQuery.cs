namespace Remote.Shell.Interrupt.Storehouse.Application.Features.TfPlans.Queries.GetTfPlansByFilter;

/// <summary>
/// Represents a query to retrieve TF plans based on filtering criteria.
/// </summary>
/// <param name="Parameters">The request parameters containing filtering and pagination settings.</param>
public record GetTfPlansByFilterQuery(RequestParameters Parameters) 
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
    var parameters = request.Parameters;
    var filterExpr = queryFilterParser.ParseFilters<TfPlan>(parameters.Filters);

    var baseSpec = BuildSpecification(filterExpr);
    var countSpec = baseSpec.Clone();

    var (pageNumber, pageSize) = GetPaginationValues(parameters);

    if (request.Parameters.EnablePagination)
      baseSpec.WithPagination(pageNumber,
                              pageSize);

    var tfPlans = await locBillUnitOfWork.TfPlans
                                         .GetManyShortAsync(baseSpec,
                                                            cancellationToken);

    if (!tfPlans.Any())
      return CreateEmptyPagedResult();
    //return new PagedList<TfPlanDTO>([], 0, 0, 0);

    var count = await locBillUnitOfWork.TfPlans
                                       .GetCountAsync(countSpec,
                                                      cancellationToken);

    var result = mapper.Map<IEnumerable<TfPlanDTO>>(tfPlans);

    return new PagedList<TfPlanDTO>(result, count, pageNumber, pageSize);
  }

  /// <summary>
  /// Builds the specification by applying filtering criteria.
  /// </summary>
  /// <param name="filterExpr">The filter expression to apply.</param>
  /// <returns>An updated specification with filtering applied.</returns>
  ITfPlanSpecification BuildSpecification(Expression<Func<TfPlan, bool>>? filterExpr)
  {
    if (filterExpr is not null)
      specification.AddFilter(filterExpr);

    return specification;
  }

  /// <summary>
  /// Extracts pagination values from the request parameters,
  /// applying default values if pagination settings are not provided.
  /// </summary>
  /// <param name="parameters">The request parameters containing pagination options.</param>
  /// <returns>
  /// A tuple containing the page number and page size.
  /// Defaults to 0 for both values if not specified in the parameters.
  /// </returns>
  static (int PageNumber, int PageSize) GetPaginationValues(RequestParameters parameters)
    => new(parameters.PageNumber ?? 0, parameters.PageSize ?? 0);
    
  /// <summary>
  /// Creates an empty paginated list of <see cref="SPRVlanDTO"/> objects.
  /// </summary>
  /// <returns>
  /// A <see cref="PagedList{T}"/> containing an empty result set with zero total count and pagination values set to 0.
  /// </returns>
  static PagedList<TfPlanDTO> CreateEmptyPagedResult()
    => new([], 0, 0, 0);
}
