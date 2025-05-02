namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.CreateGate;

/// <summary>
/// Represents a command to create a new gate entity.
/// </summary>
/// <param name="CreateGateDTO">The data transfer object containing gate details.</param>
public record CreateGateCommand(CreateGateDTO CreateGateDTO) : ICommand<Unit>;

/// <summary>
/// Handles the CreateGateCommand and creates a new gate entity.
/// </summary>
/// <remarks>
/// This handler validates whether a gate with the same IP address already exists,
/// and if not, creates and inserts a new gate entity into the database.
/// </remarks>
/// <param name="gateUnitOfWork">Unit of work for gate-related database operations.</param>
/// <param name="specification">Gate specification used for filtering.</param>
/// <param name="queryFilterParser">Parser for processing filter expressions.</param>
/// <param name="mapper">Object mapper for DTO transformation.</param>
internal class CreateGateCommandHandler(IGateUnitOfWork gateUnitOfWork,
                                        IGateSpecification specification,
                                        IQueryFilterParser queryFilterParser,
                                        IMapper mapper)
  : ICommandHandler<CreateGateCommand, Unit>
{
  /// <summary>
  /// Handles the request to create a new gate entity.
  /// </summary>
  /// <param name="request">The command request containing gate details.</param>
  /// <param name="cancellationToken">Token that allows operation cancellation.</param>
  /// <returns>A unit value indicating successful execution.</returns>
  public async Task<Unit> Handle(CreateGateCommand request,
                                 CancellationToken cancellationToken)
  {
    // Create request filter with the provided IPAddress
    var requestParameters = new RequestParameters
    {
      Filters = [
        new ()
        {
          PropertyPath = "IPAddress",
          Operator = FilterOperator.Equals,
          Value = request.CreateGateDTO.IPAddress
        }
      ]
    };

    // Parse the filter expression
    var filterExpr = queryFilterParser.ParseFilters<Gate>(requestParameters.Filters);

    // Build the base specification with filtering applied
    var baseSpec = BuildSpecification(specification,
                                      filterExpr);

    // Check if a gate with the given IPAddress already exists
    var existing = await gateUnitOfWork.Gates
                                       .AnyByQueryAsync(baseSpec,
                                                        cancellationToken);
    
    // If a matching gate exists, throw an exception
    if (existing)
      throw new EntityAlreadyExists(typeof(Gate),
                                    filterExpr is not null ? filterExpr.ToString() : string.Empty);

    // Map the request DTO to a new Gate entity
    var gate = mapper.Map<Gate>(request.CreateGateDTO);

    // Insert the new entity into the database
    gateUnitOfWork.Gates.InsertOne(gate);

    // Commit the transaction
    gateUnitOfWork.Complete();

    // Return a unit value indicating completion
    return Unit.Value;
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
