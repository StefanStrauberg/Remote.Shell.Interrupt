namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Queries.GetGateById;

/// <summary>
/// Represents a query to retrieve a gate by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the gate to retrieve.</param>
public record GetGateByIdQuery(Guid Id) : IQuery<GateDTO>;

/// <summary>
/// Handles the GetGateByIdQuery and retrieves the specified gate.
/// </summary>
/// <remarks>
/// This handler ensures that the requested gate exists, retrieves its data, and
/// returns a mapped result as a DTO.
/// </remarks>
/// <param name="gateUnitOfWork">Unit of work for gate-related database operations.</param>
/// <param name="specification">Gate specification used for filtering.</param>
/// <param name="queryFilterParser">Parser for processing filter expressions.</param>
/// <param name="mapper">Object mapper for DTO transformation.</param>
internal class GetGateByIdQueryHandler(IGateUnitOfWork gateUnitOfWork,
                                       IGateSpecification specification,
                                       IQueryFilterParser queryFilterParser,
                                       IMapper mapper)
  : IQueryHandler<GetGateByIdQuery, GateDTO>
{
  /// <summary>
  /// Handles the request to retrieve a gate by its ID.
  /// </summary>
  /// <param name="request">The query containing the gate ID.</param>
  /// <param name="cancellationToken">Token to handle request cancellation.</param>
  /// <returns>A DTO representing the gate.</returns>
  async Task<GateDTO> IRequestHandler<GetGateByIdQuery, GateDTO>.Handle(GetGateByIdQuery request,
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
    var filterExpr = queryFilterParser.ParseFilters<Gate>(requestParameters.Filters);

    // Build the base specification with filtering applied
    var baseSpec = BuildSpecification(specification,
                                      filterExpr);

    // Check if a gate with the given ID already exists
    var existing = await gateUnitOfWork.Gates
                                       .AnyByQueryAsync(baseSpec,
                                                        cancellationToken);
    
    // If a matching gate doesn't exists, throw an exception
    if (!existing)
      throw new EntityNotFoundException(typeof(Gate),
                                        filterExpr is not null ? filterExpr.ToString() : string.Empty);

    // Retrieve entity
    var gate = await gateUnitOfWork.Gates
                                   .GetOneShortAsync(baseSpec,
                                                     cancellationToken);

    // Map the retrieved data to the DTO
    var result = mapper.Map<GateDTO>(gate);

    // Return the mapped result
    return result;
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
