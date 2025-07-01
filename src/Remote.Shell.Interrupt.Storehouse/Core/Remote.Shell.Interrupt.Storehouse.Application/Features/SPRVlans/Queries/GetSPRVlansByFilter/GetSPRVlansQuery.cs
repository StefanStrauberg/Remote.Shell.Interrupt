namespace Remote.Shell.Interrupt.Storehouse.Application.Features.SPRVlans.Queries.GetSPRVlansByFilter;

/// <summary>
/// Represents a query to retrieve SPR VLANs based on filtering criteria.
/// </summary>
/// <param name="Parameters">The request parameters containing filtering and pagination settings.</param>
public sealed record GetSPRVlansByFilterQuery(RequestParameters Parameters) 
  : IQuery<PagedList<SPRVlanDTO>>;

/// <summary>
/// Handles the GetSPRVlansByFilterQuery and retrieves filtered SPR VLANs.
/// </summary>
/// <remarks>
/// This handler applies filtering expressions, builds the necessary specifications,
/// manages pagination, retrieves SPR VLANs, and returns the mapped results.
/// </remarks>
/// <param name="locBillUnitOfWork">Unit of work for database operations.</param>
/// <param name="specification">Specification used for filtering SPR VLAN entities.</param>
/// <param name="filterParser">Parser for processing filter expressions.</param>
/// <param name="mapper">Object mapper for DTO transformation.</param>
internal class GetSPRVlansByFilterQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                               ISPRVlanSpecification specification,
                                               IQueryFilterParser filterParser,
                                               IMapper mapper)
  : IQueryHandler<GetSPRVlansByFilterQuery, PagedList<SPRVlanDTO>>
{
  /// <summary>
  /// Handles the request to retrieve SPR VLANs based on filtering criteria.
  /// </summary>
  /// <param name="request">The query request containing filtering and pagination parameters.</param>
  /// <param name="cancellationToken">Token to support request cancellation.</param>
  /// <returns>A paginated list of SPR VLAN DTOs.</returns>
  async Task<PagedList<SPRVlanDTO>> IRequestHandler<GetSPRVlansByFilterQuery, PagedList<SPRVlanDTO>>.Handle(GetSPRVlansByFilterQuery request,
                                                                                                            CancellationToken cancellationToken)
  {
    var parameters = request.Parameters;
    var filterExpr = filterParser.ParseFilters<SPRVlan>(parameters.Filters);

    var filteringSpec = BuildSpecification(filterExpr);
    var countingSpec = filteringSpec.Clone();

    var (pageNumber, pageSize) = GetPaginationValues(parameters);

    if (parameters.EnablePagination)
      filteringSpec.WithPagination(pageNumber, pageSize);

    var sprVlans = await locBillUnitOfWork.SPRVlans
                                          .GetManyShortAsync(filteringSpec, cancellationToken);

    if (!sprVlans.Any())
      return CreateEmptyPagedResult();

    var totalCount = await locBillUnitOfWork.SPRVlans
                                            .GetCountAsync(countingSpec, cancellationToken);

    var dtoList = mapper.Map<List<SPRVlanDTO>>(sprVlans);

    return new PagedList<SPRVlanDTO>(dtoList, totalCount, pageNumber, pageSize);
  }

  /// <summary>
  /// Builds the specification by applying filtering criteria.
  /// </summary>
  /// <param name="filterExpr">The filter expression to apply.</param>
  /// <returns>An updated specification with filtering applied.</returns>
  ISPRVlanSpecification BuildSpecification(Expression<Func<SPRVlan, bool>>? filterExpr)
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
  static PagedList<SPRVlanDTO> CreateEmptyPagedResult()
    => new([], 0, 0, 0);
}
