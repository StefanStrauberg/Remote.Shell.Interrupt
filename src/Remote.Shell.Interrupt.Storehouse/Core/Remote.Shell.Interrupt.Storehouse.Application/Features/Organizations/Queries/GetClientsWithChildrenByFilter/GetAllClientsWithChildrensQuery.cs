namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientsWithChildrenByFilter;

/// <summary>
/// Represents a query to fetch clients along with their related child entities
/// based on filtering criteria.
/// </summary>
/// <param name="RequestParameters">The request parameters containing filtering and pagination settings.</param>
public record GetClientsWithChildrenByFilterQuery(RequestParameters RequestParameters) 
  : IQuery<PagedList<DetailClientDTO>>;

/// <summary>
/// Handles the GetClientsWithChildrenByFilterQuery and retrieves clients
/// along with their child entities based on specified filters.
/// </summary>
/// <remarks>
/// This handler applies filtering expressions, builds the necessary specifications,
/// handles pagination, retrieves related child data, and returns mapped results.
/// </remarks>
/// <param name="locBillUnitOfWork">Unit of work for database operations.</param>
/// <param name="specification">Client specification used for filtering.</param>
/// <param name="queryFilterParser">Parser for processing filter expressions.</param>
/// <param name="mapper">Object mapper for DTO transformation.</param>
internal class GetClientsWithChildrenByFilterQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                                          IClientSpecification specification,
                                                          IQueryFilterParser queryFilterParser,
                                                          IMapper mapper)
  : IQueryHandler<GetClientsWithChildrenByFilterQuery, PagedList<DetailClientDTO>>
{
  /// <summary>
  /// Handles the request to retrieve clients along with their child entities
  /// based on filter conditions.
  /// </summary>
  /// <param name="request">The query request containing filtering and pagination parameters.</param>
  /// <param name="cancellationToken">Token for request cancellation support.</param>
  /// <returns>A paged list of detailed client DTOs.</returns>
  async Task<PagedList<DetailClientDTO>> IRequestHandler<GetClientsWithChildrenByFilterQuery, PagedList<DetailClientDTO>>.Handle(GetClientsWithChildrenByFilterQuery request,
                                                                                                                                 CancellationToken cancellationToken)
  {
    // // Parse the filter expression
    // var filterExpr = queryFilterParser.ParseFilters<Client>(request.RequestParameters
    //                                                                .Filters);

    // // Build the base specification with filtering applied
    // var baseSpec = BuildSpecification(specification,
    //                                   filterExpr);

    // // Create a specification for counting total matching records
    // var countSpec = baseSpec.Clone();

    // // Extract pagination parameters
    // var pageNumber = request.RequestParameters.PageNumber ?? 0;
    // var pageSize = request.RequestParameters.PageSize ?? 0;

    // // Apply pagination settings if enabled
    // if (request.RequestParameters.IsPaginated)
    //     baseSpec.ConfigurePagination(pageNumber,
    //                             pageSize);

    // // Retrieve data, including related entities
    // var clients = await locBillUnitOfWork.Clients
    //                                      .GetManyWithChildrenAsync(baseSpec,
    //                                                                cancellationToken);

    // // Return an empty paginated list if no data are found.
    // if (!clients.Any())
    //   return new PagedList<DetailClientDTO>([],0,0,0);

    // // Retrieve the total count of matching records
    // var count = await locBillUnitOfWork.Clients
    //                                    .GetCountAsync(countSpec,
    //                                                   cancellationToken);

    // // Map the retrieved data to the DTO
    // var result = mapper.Map<IEnumerable<DetailClientDTO>>(clients);

    // // Return the mapped result
    // return new PagedList<DetailClientDTO>(result,
    //                                       count,
    //                                       pageNumber,
    //                                       pageSize);
    return await Task.FromResult<PagedList<DetailClientDTO>>(new PagedList<DetailClientDTO>([],0,new(0,0)));
  }

  /// <summary>
  /// Builds the specification by applying filtering and includes related entities.
  /// </summary>
  /// <param name="baseSpec">The base client specification.</param>
  /// <param name="filterExpr">The filter expression to apply.</param>
  /// <returns>An updated specification with filtering and includes applied.</returns>
  // static IClientSpecification BuildSpecification(IClientSpecification baseSpec,
  //                                                Expression<Func<Client, bool>>? filterExpr)
  // {
  //   var spec = baseSpec.AddInclude(c => c.COD)
  //                      .AddInclude(c => c.TfPlan!)
  //                      .AddInclude(c => c.SPRVlans);

  //   if (filterExpr is not null)
  //     spec.AddFilter(filterExpr);

  //   return (IClientSpecification)spec;
  // }
}
