namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientById;

/// <summary>
/// Represents a query to retrieve a client by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the client.</param>
public record GetClientByIdQuery(Guid Id) : IQuery<DetailClientDTO>;

/// <summary>
/// Handles the GetClientByIdQuery and retrieves the corresponding client.
/// </summary>
/// <remarks>
/// This handler applies filtering based on the provided client ID,
/// checks if the client exists, retrieves the necessary data with related entities,
/// and returns the mapped result.
/// </remarks>
/// <param name="locBillUnitOfWork">Unit of work for database operations.</param>
/// <param name="specification">Client specification used for filtering.</param>
/// <param name="queryFilterParser">Parser for processing filter expressions.</param>
/// <param name="mapper">Object mapper for DTO transformation.</param>
internal class GetClientByIdQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                         IClientSpecification specification,
                                         IQueryFilterParser queryFilterParser,
                                         IMapper mapper) 
  : IQueryHandler<GetClientByIdQuery, DetailClientDTO>
{
  /// <summary>
  /// Handles the request to retrieve a client by its ID.
  /// </summary>
  /// <param name="request">The query request containing the client ID.</param>
  /// <param name="cancellationToken">Token for request cancellation support.</param>
  /// <returns>A detailed client DTO corresponding to the provided ID.</returns>
  async Task<DetailClientDTO> IRequestHandler<GetClientByIdQuery, DetailClientDTO>.Handle(GetClientByIdQuery request,
                                                                                          CancellationToken cancellationToken)
  {
    // Create request filter with the provided ID
    var requestParameters = new RequestParameters
    {
      Filters = [
        new ()
        {
          PropertyPath = "Id",
          Operator = FilterOperator.Equals,
          Value = request.Id.ToString()
        }
      ]
    };

    // Parse the filter expression
    var filterExpr = queryFilterParser.ParseFilters<Client>(requestParameters.Filters);

    // Build the base specification with filtering applied
    var baseSpec = BuildSpecification(specification,
                                      filterExpr);

    // Check if a client with the given ID already exists
    var existing = await locBillUnitOfWork.Clients
                                          .AnyByQueryAsync(baseSpec,
                                                           cancellationToken);

    // If a matching client doesn't exists, throw an exception
    if (!existing)
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