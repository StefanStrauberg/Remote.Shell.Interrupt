namespace Remote.Shell.Interrupt.Storehouse.Application.Features.TfPlans.Queries.GetTfPlansByFilter;

/// <summary>
/// Represents a query to retrieve TF plans based on filtering criteria and pagination settings.
/// </summary>
/// <param name="Parameters">The filtering and pagination parameters provided in the request.</param>
public record GetTfPlansByFilterQuery(RequestParameters Parameters) 
    : IQuery<PagedList<TfPlanDTO>>;

/// <summary>
/// Handles the <see cref="GetTfPlansByFilterQuery"/> and returns a paginated list of TF plan DTOs
/// based on filter expressions and pagination settings.
/// </summary>
/// <param name="locBillUnitOfWork">The unit of work for accessing TF Plan repositories.</param>
/// <param name="specification">The base specification to build query filters on top of.</param>
/// <param name="queryFilterParser">The service that parses query filters into expressions.</param>
/// <param name="mapper">The object mapper used to convert domain entities to DTOs.</param>
internal class GetTfPlansByFilterQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                              ITfPlanSpecification specification,
                                              IQueryFilterParser queryFilterParser,
                                              IMapper mapper)
    : IQueryHandler<GetTfPlansByFilterQuery, PagedList<TfPlanDTO>>
{
  /// <summary>
  /// Processes the incoming TF plan query, applies filters and pagination,
  /// and returns a paginated result of matching TF plan DTOs.
  /// </summary>
  /// <param name="request">The incoming request containing filter and pagination parameters.</param>
  /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
  /// <returns>A paginated list of <see cref="TfPlanDTO"/>s.</returns>
  async Task<PagedList<TfPlanDTO>> IRequestHandler<GetTfPlansByFilterQuery, PagedList<TfPlanDTO>>.Handle(GetTfPlansByFilterQuery request,
                                                                                                         CancellationToken cancellationToken)
  {
    var specification = BuildSpecification(request.Parameters);
    var pagination = BuildPagination(request.Parameters);

    if (request.Parameters.IsPaginated)
      specification.ConfigurePagination(pagination);

    var tfPlans = await FetchTfPlansAsync(specification, cancellationToken);

    if (IsEmptyResult(tfPlans))
      return PagedList<TfPlanDTO>.Empty();

    var totalCount = await CountResultsAsync(specification, cancellationToken);
    var tfPlanDtos = MapToTfPlanDtos(tfPlans);

    return PagedList<TfPlanDTO>.Create(tfPlanDtos, totalCount, pagination);
  }
  
  /// <summary>
  /// Builds a filtering specification by applying parsed filter expressions
  /// from the incoming request parameters to a cloned base specification.
  /// </summary>
  /// <param name="parameters">The request parameters containing filtering instructions.</param>
  /// <returns>A configured filtering specification.</returns>
  ISpecification<TfPlan> BuildSpecification(RequestParameters parameters)
  {
    var filterExpr = queryFilterParser.ParseFilters<TfPlan>(parameters.Filters);
    var spec = specification.Clone();

    if (filterExpr is not null)
      spec.AddFilter(filterExpr);

    return spec;
  }

  /// <summary>
  /// Creates a <see cref="PaginationContext"/> based on the provided request parameters.
  /// Defaults to 0 if no values are specified.
  /// </summary>
  /// <param name="parameters">The pagination settings from the request.</param>
  /// <returns>A fully formed <see cref="PaginationContext"/>.</returns>
  static PaginationContext BuildPagination(RequestParameters parameters)
    => new(parameters.PageNumber ?? 0, parameters.PageSize ?? 0);

  /// <summary>
  /// Retrieves a filtered collection of <see cref="TfPlan"/> entities from the data store.
  /// </summary>
  /// <param name="spec">The filtering specification to apply.</param>
  /// <param name="cancellationToken">Token to support request cancellation.</param>
  /// <returns>A list of matching <see cref="TfPlan"/> entities.</returns>    
  async Task<IEnumerable<TfPlan>> FetchTfPlansAsync(ISpecification<TfPlan> spec, CancellationToken cancellationToken)
    => await locBillUnitOfWork.TfPlans.GetManyShortAsync(spec, cancellationToken);

  /// <summary>
  /// Counts the total number of <see cref="TfPlan"/> entities matching the specified criteria.
  /// </summary>
  /// <param name="spec">The specification used for counting matches.</param>
  /// <param name="cancellationToken">Token to support request cancellation.</param>
  /// <returns>The total number of matching records.</returns>
  async Task<int> CountResultsAsync(ISpecification<TfPlan> spec, CancellationToken cancellationToken)
    => await locBillUnitOfWork.TfPlans.GetCountAsync(spec, cancellationToken);

  /// <summary>
  /// Maps a collection of <see cref="TfPlan"/> domain entities to a collection of DTOs.
  /// </summary>
  /// <param name="entities">The domain entities to convert.</param>
  /// <returns>The corresponding <see cref="TfPlanDTO"/> representations.</returns>
  IEnumerable<TfPlanDTO> MapToTfPlanDtos(IEnumerable<TfPlan> entities)
    => mapper.Map<IEnumerable<TfPlanDTO>>(entities);

  /// <summary>
  /// Determines whether a given collection of <see cref="TfPlan"/> entities is empty or null.
  /// </summary>
  /// <param name="tfPlans">The collection of TF plans to inspect.</param>
  /// <returns><c>true</c> if the collection is null or contains no items; otherwise, <c>false</c>.</returns>
  static bool IsEmptyResult(IEnumerable<TfPlan> tfPlans)
    => tfPlans == null || !tfPlans.Any();
}
