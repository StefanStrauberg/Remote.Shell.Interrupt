namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientWithChildrenByFilter;

/// <summary>
/// Represents a query to retrieve a client along with its related child entities
/// based on filtering criteria.
/// </summary>
/// <param name="RequestParameters">The request parameters containing filtering criteria.</param>
public record GetClientWithChildrenByFilterQuery(RequestParameters RequestParameters) : IRequest<DetailClientDTO>;

/// <summary>
/// Handles the GetClientWithChildrenByFilterQuery and retrieves the requested client
/// along with its related child entities.
/// </summary>
/// <remarks>
/// This handler applies filtering criteria, verifies the client's existence,
/// retrieves related data, and returns the mapped result.
/// </remarks>
/// <param name="locBillUnitOfWork">Unit of work for database operations.</param>
/// <param name="specification">Client specification used for filtering.</param>
/// <param name="queryFilterParser">Parser for processing filter expressions.</param>
/// <param name="mapper">Object mapper for DTO transformation.</param>
internal class GetClientWithChildrenByFilterHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                                    IClientSpecification specification,
                                                    IQueryFilterParser queryFilterParser,
                                                    IMapper mapper) 
  : IRequestHandler<GetClientWithChildrenByFilterQuery, DetailClientDTO>
{
  /// <summary>
  /// Handles the request to retrieve a client along with its child entities
  /// based on filtering conditions.
  /// </summary>
  /// <param name="request">The query request containing filtering parameters.</param>
  /// <param name="cancellationToken">Token that allows operation cancellation.</param>
  /// <returns>A detailed client DTO including its related child entities.</returns>
  async Task<DetailClientDTO> IRequestHandler<GetClientWithChildrenByFilterQuery, DetailClientDTO>.Handle(GetClientWithChildrenByFilterQuery request,
                                                                                                          CancellationToken cancellationToken)
  {
    // Parse the filter expression
    var filterExpr = queryFilterParser.ParseFilters<Client>(request.RequestParameters
                                                                   .Filters);

    // Build the base specification with filtering applied
    var baseSpec = BuildSpecification(specification,
                                      filterExpr);

    // Check if a client with the filtering already exists
    var existingClient = await locBillUnitOfWork.Clients
                                                .AnyByQueryAsync(baseSpec,
                                                                 cancellationToken);

    // If a matching client doesn't exists, throw an exception
    if (!existingClient)
      throw new EntityNotFoundException(typeof(Client),
                                        filterExpr is not null ? filterExpr.ToString() : string.Empty);

    // Retrieve data, including related entities
    var client = await locBillUnitOfWork.Clients
                                        .GetOneWithChildrenAsync(baseSpec,
                                                                  cancellationToken);
    
    // Map the retrieved data to the DTO
    var result = mapper.Map<DetailClientDTO>(client);
    
    // Return the mapped result
    return result;
  }

  /// <summary>
  /// Builds the specification by applying filtering and includes related entities.
  /// </summary>
  /// <param name="baseSpec">The base client specification.</param>
  /// <param name="filterExpr">The filter expression to apply.</param>
  /// <returns>An updated specification with filtering and includes applied.</returns>
  static IClientSpecification BuildSpecification(IClientSpecification baseSpec,
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