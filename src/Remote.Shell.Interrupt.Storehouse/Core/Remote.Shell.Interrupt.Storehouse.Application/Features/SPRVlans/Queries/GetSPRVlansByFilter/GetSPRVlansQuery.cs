namespace Remote.Shell.Interrupt.Storehouse.Application.Features.SPRVlans.Queries.GetSPRVlansByFilter;

/// <summary>
/// Represents a query for retrieving SPR VLANs based on provided filtering and pagination parameters.
/// </summary>
/// <param name="Parameters">Filtering and pagination settings for the query.</param>
public sealed record GetSPRVlansByFilterQuery(RequestParameters Parameters) 
  : IQuery<PagedList<SPRVlanDTO>>;

/// <summary>
/// Handles the <see cref="GetSPRVlansByFilterQuery"/> by applying filters and pagination settings
/// to retrieve a paginated list of SPR VLAN data.
/// </summary>
/// <param name="locBillUnitOfWork">The unit of work for accessing SPR VLAN repositories.</param>
/// <param name="specification">The base specification to build query filters on top of.</param>
/// <param name="queryFilterParser">The service that parses query filters into expressions.</param>
/// <param name="mapper">The object mapper used to convert domain entities to DTOs.</param>
internal class GetSPRVlansByFilterQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                               ISPRVlanSpecification specification,
                                               IQueryFilterParser queryFilterParser,
                                               IMapper mapper)
  : IQueryHandler<GetSPRVlansByFilterQuery, PagedList<SPRVlanDTO>>
{
  /// <summary>
  /// Handles the incoming request by filtering, paginating, retrieving,
  /// and mapping SPR VLAN entities to DTOs.
  /// </summary>
  /// <param name="request">The query containing filtering and pagination parameters.</param>
  /// <param name="cancellationToken">Token to observe while waiting for the operation to complete.</param>
  /// <returns>A paginated list of <see cref="SPRVlanDTO"/> objects.</returns>
  async Task<PagedList<SPRVlanDTO>> IRequestHandler<GetSPRVlansByFilterQuery, PagedList<SPRVlanDTO>>.Handle(GetSPRVlansByFilterQuery request,
                                                                                                            CancellationToken cancellationToken)
  {
    var filter = BuildFilteringSpec(request.Parameters);
    var pagination = BuildPaginationSpec(request.Parameters);

    if (request.Parameters.IsPaginated)
      filter.ConfigurePagination(pagination);

    var sprVlans = await FetchSPRVlansAsync(filter, cancellationToken);

    if (NoResultsFound(sprVlans))
      return EmptyResult.GetFor<SPRVlanDTO>();

    var total = await CountResultsAsync(filter, cancellationToken);
    var dtos = MapToDto(sprVlans);

    return CreatePagedResult(dtos, total, pagination);
  }

  /// <summary>
  /// Builds a filtered specification for querying SPR VLANs based on the request parameters.
  /// </summary>
  /// <param name="parameters">The filtering parameters provided in the query request.</param>
  /// <returns>A filtering specification for SPR VLAN entities.</returns>
  private ISpecification<SPRVlan> BuildFilteringSpec(RequestParameters parameters)
  {
    var filterExpr = queryFilterParser.ParseFilters<SPRVlan>(parameters.Filters);
    var spec = specification.Clone();

    if (filterExpr is not null)
      spec.AddFilter(filterExpr);

    return spec;
  }

  /// <summary>
  /// Constructs a pagination context using the pagination information from the request.
  /// </summary>
  /// <param name="parameters">The pagination-related request parameters.</param>
  /// <returns>A <see cref="PaginationContext"/> with calculated page number and size.</returns>
  static PaginationContext BuildPaginationSpec(RequestParameters parameters)
    => new(parameters.PageNumber ?? 0, parameters.PageSize ?? 0);

  /// <summary>
  /// Retrieves a list of SPR VLANs from the data source that match the specified filter.
  /// </summary>
  /// <param name="spec">The filter specification used to narrow the result set.</param>
  /// <param name="cancellationToken">Cancellation token to stop the operation if requested.</param>
  /// <returns>A filtered collection of <see cref="SPRVlan"/> domain entities.</returns>
  async Task<IEnumerable<SPRVlan>> FetchSPRVlansAsync(ISpecification<SPRVlan> spec, CancellationToken cancellationToken)
    => await locBillUnitOfWork.SPRVlans.GetManyShortAsync(spec, cancellationToken);

  /// <summary>
  /// Calculates the total number of SPR VLAN records that match the provided filter specification.
  /// </summary>
  /// <param name="spec">The filter specification to count against.</param>
  /// <param name="cancellationToken">Token to cancel the operation.</param>
  /// <returns>The total count of matching SPR VLAN records.</returns>
  async Task<int> CountResultsAsync(ISpecification<SPRVlan> spec, CancellationToken cancellationToken)
    => await locBillUnitOfWork.SPRVlans.GetCountAsync(spec, cancellationToken);

  /// <summary>
  /// Maps a collection of <see cref="SPRVlan"/> domain entities to their DTO representations.
  /// </summary>
  /// <param name="entities">The list of domain entities to convert.</param>
  /// <returns>A collection of <see cref="SPRVlanDTO"/>s.</returns>
  IEnumerable<SPRVlanDTO> MapToDto(IEnumerable<SPRVlan> entities)
    => mapper.Map<IEnumerable<SPRVlanDTO>>(entities);

  /// <summary>
  /// Checks whether the collection of SPR VLANs is null or contains no elements.
  /// </summary>
  /// <param name="sprVlans">The collection of SPR VLANs to evaluate.</param>
  /// <returns><c>true</c> if there are no results; otherwise, <c>false</c>.</returns>
  static bool NoResultsFound(IEnumerable<SPRVlan> sprVlans)
    => sprVlans == null || !sprVlans.Any();

  /// <summary>
  /// Wraps the mapped DTOs and pagination metadata into a paged result structure.
  /// </summary>
  /// <typeparam name="TResult">The type of items contained in the result.</typeparam>
  /// <param name="dtoList">The list of DTOs to include in the paginated result.</param>
  /// <param name="totalAmount">The total number of records matching the filter.</param>
  /// <param name="paginationContext">Pagination metadata used to format the result.</param>
  /// <returns>A <see cref="PagedList{T}"/> containing the paginated response.</returns>
  static PagedList<TResult> CreatePagedResult<TResult>(IEnumerable<TResult> dtoList,
                                                       int totalAmount,
                                                       PaginationContext paginationContext)
    => new(dtoList, totalAmount, paginationContext);
}
