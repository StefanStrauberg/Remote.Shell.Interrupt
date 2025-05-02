namespace Remote.Shell.Interrupt.Storehouse.Application.Features.SPRVlans.Queries.GetSPRVlansByFilter;

/// <summary>
/// Represents a query to retrieve SPR VLANs based on filtering criteria.
/// </summary>
/// <param name="RequestParameters">The request parameters containing filtering and pagination settings.</param>
public record GetSPRVlansByFilterQuery(RequestParameters RequestParameters) 
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
/// <param name="queryFilterParser">Parser for processing filter expressions.</param>
/// <param name="mapper">Object mapper for DTO transformation.</param>
internal class GetSPRVlansByFilterQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                               ISPRVlanSpecification specification,
                                               IQueryFilterParser queryFilterParser,
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
    // Parse the filter expression
    var filterExpr = queryFilterParser.ParseFilters<SPRVlan>(request.RequestParameters
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
    var sprVlans = await locBillUnitOfWork.SPRVlans
                                          .GetManyShortAsync(baseSpec,
                                                             cancellationToken);
                                                             
    // Return an empty paginated list if no data are found.
    if (!sprVlans.Any())
      return new PagedList<SPRVlanDTO>([],0,0,0);

    // Retrieve the total count of matching records
    var count = await locBillUnitOfWork.SPRVlans
                                       .GetCountAsync(countSpec,
                                                      cancellationToken);

    // Map the retrieved data to the DTO
    var result = mapper.Map<List<SPRVlanDTO>>(sprVlans);

    // Return the mapped result
    return new PagedList<SPRVlanDTO>(result,
                                     count,
                                     pageNumber,
                                     pageSize);
  }

  /// <summary>
  /// Builds the specification by applying filtering criteria.
  /// </summary>
  /// <param name="baseSpec">The base SPR VLAN specification.</param>
  /// <param name="filterExpr">The filter expression to apply.</param>
  /// <returns>An updated specification with filtering applied.</returns>
  static ISPRVlanSpecification BuildSpecification(ISPRVlanSpecification baseSpec,
                                                  Expression<Func<SPRVlan, bool>>? filterExpr)
  {
    var spec = baseSpec;

    if (filterExpr is not null)
        spec.AddFilter(filterExpr);

    return spec;
  }
}
