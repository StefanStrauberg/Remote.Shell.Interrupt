namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetAll;

/// <summary>
/// Represents a query for retrieving all clients with their child entities.
/// </summary>
/// <param name="RequestParameters">
/// The request parameters, including pagination and filters.
/// </param>
public record GetAllClientsWithChildrensQuery(RequestParametersUpdated RequestParameters) 
  : IQuery<PagedList<DetailClientDTO>>;

/// <summary>
/// Handles the processing of the <see cref="GetAllClientsWithChildrensQuery"/> query.
/// </summary>
internal class GetAllClientsWithChildrensQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                                      IClientSpecification clientSpecification,
                                                      IQueryFilterParser queryFilterParser,
                                                      IMapper mapper)
  : IQueryHandler<GetAllClientsWithChildrensQuery, PagedList<DetailClientDTO>>
{
  /// <summary>
  /// Processes the query to retrieve paginated and filtered clients with their children.
  /// </summary>
  /// <param name="request">The query request object.</param>
  /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
  /// <returns>
  /// A paginated list of client details, including children, after applying filters and pagination.
  /// </returns>
  async Task<PagedList<DetailClientDTO>> IRequestHandler<GetAllClientsWithChildrensQuery, PagedList<DetailClientDTO>>.Handle(GetAllClientsWithChildrensQuery request,
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
    
    // Retrieve data
    var clients = await locBillUnitOfWork.Clients
                                         .GetManyWithChildrenAsync(baseSpec,
                                                                   cancellationToken);
    
    // Map results
    var result = mapper.Map<IEnumerable<DetailClientDTO>>(clients);

    // Return results
    return new PagedList<DetailClientDTO>(result,
                                          count,
                                          pageNumber,
                                          pageSize);
  }

  /// <summary>
  /// Builds the specification with included entities and filters.
  /// </summary>
  /// <param name="baseSpec">The base client specification.</param>
  /// <param name="filterExpr">The filter expression to apply.</param>
  /// <returns>
  /// An updated client specification including related entities and filters.
  /// </returns>
  private static IClientSpecification BuildSpecification(IClientSpecification baseSpec,
                                                         Expression<Func<Client, bool>>? filterExpr)
  {
    var spec = baseSpec.AddInclude(c => c.COD)
                       .AddInclude(c => c.TfPlan!)
                       .AddInclude(c => c.SPRVlans);

    if (filterExpr is not null)
        spec.AddFilter(filterExpr);

    return (IClientSpecification)spec;
  }
}
