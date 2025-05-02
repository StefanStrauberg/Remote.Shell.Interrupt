namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Queries.GetGatesByFilter;

/// <summary>
/// Represents a query to retrieve gates based on filtering criteria.
/// </summary>
/// <param name="RequestParameters">The request parameters containing filtering and pagination settings.</param>
public record GetGatesByFilterQuery(RequestParameters RequestParameters) 
  : IQuery<PagedList<GateDTO>>;

/// <summary>
/// Handles the GetGatesByFilterQuery and retrieves filtered gates.
/// </summary>
/// <remarks>
/// This handler applies filtering expressions, builds the necessary specifications,
/// handles pagination, retrieves gates, and returns the mapped results.
/// </remarks>
/// <param name="gateUnitOfWork">Unit of work for gate-related database operations.</param>
/// <param name="specification">Specification used for filtering gate entities.</param>
/// <param name="queryFilterParser">Parser for processing filter expressions.</param>
/// <param name="mapper">Object mapper for DTO transformation.</param>
internal class GetGatesByFilterQueryHandler(IGateUnitOfWork gateUnitOfWork,
                                            IGateSpecification specification,
                                            IQueryFilterParser queryFilterParser,
                                            IMapper mapper)
  : IQueryHandler<GetGatesByFilterQuery, PagedList<GateDTO>>
{
  /// <summary>
  /// Handles the request to retrieve gates based on filter conditions.
  /// </summary>
  /// <param name="request">The query request containing filtering and pagination parameters.</param>
  /// <param name="cancellationToken">Token to support request cancellation.</param>
  /// <returns>A paginated list of gate DTOs.</returns>
  async Task<PagedList<GateDTO>> IRequestHandler<GetGatesByFilterQuery, PagedList<GateDTO>>.Handle(GetGatesByFilterQuery request,
                                                                                                   CancellationToken cancellationToken)
  {
    // Parse the filter expression
    var filterExpr = queryFilterParser.ParseFilters<Gate>(request.RequestParameters
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
    var gates = await gateUnitOfWork.Gates
                                    .GetManyShortAsync(baseSpec,
                                                       cancellationToken);

    // Return an empty paginated list if no data are found.
    if (!gates.Any())
      return new PagedList<GateDTO>([],0,0,0);

    // Retrieve the total count of matching records                    
    var count = await gateUnitOfWork.Gates
                                    .GetCountAsync(countSpec,
                                                   cancellationToken);

    // Map the retrieved data to the DTO
    var result = mapper.Map<IEnumerable<GateDTO>>(gates);

    // Return the mapped result
    return new PagedList<GateDTO>(result,
                                  count,
                                  pageNumber,
                                  pageSize);
  }

  /// <summary>
  /// Builds the specification by applying filtering criteria.
  /// </summary>
  /// <param name="baseSpec">The base gate specification.</param>
  /// <param name="filterExpr">The filter expression to apply.</param>
  /// <returns>An updated specification with filtering applied.</returns>
  static IGateSpecification BuildSpecification(IGateSpecification baseSpec,
                                               Expression<Func<Gate, bool>>? filterExpr)
  {
    var spec = baseSpec;

    if (filterExpr is not null)
        spec.AddFilter(filterExpr);

    return spec;
  }
}
