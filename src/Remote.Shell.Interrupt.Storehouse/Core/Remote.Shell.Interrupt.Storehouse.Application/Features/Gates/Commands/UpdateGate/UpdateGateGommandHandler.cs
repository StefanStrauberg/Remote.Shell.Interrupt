namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.UpdateGate;

/// <summary>
/// Represents a command to update an existing gate.
/// </summary>
/// <param name="UpdateGateDTO">The data transfer object containing updated gate details.</param>
public record UpdateGateCommand(UpdateGateDTO UpdateGateDTO) : ICommand<Unit>;

/// <summary>
/// Handles the UpdateGateCommand and updates an existing gate.
/// </summary>
/// <remarks>
/// This handler ensures that the specified gate exists, applies the update, 
/// and commits the changes to the database.
/// </remarks>
/// <param name="gateUnitOfWork">Unit of work for managing gate-related database operations.</param>
/// <param name="specification">Gate specification used for filtering.</param>
/// <param name="queryFilterParser">Parser for processing filter expressions.</param>
/// <param name="mapper">Object mapper for DTO transformation.</param>
internal class UpdateGateGommandHandler(IGateUnitOfWork gateUnitOfWork,
                                        IGateSpecification specification,
                                        IQueryFilterParser queryFilterParser,
                                        IMapper mapper)
  : ICommandHandler<UpdateGateCommand, Unit>
{
  /// <summary>
  /// Handles the request to update an existing gate.
  /// </summary>
  /// <param name="request">The command containing the updated gate data.</param>
  /// <param name="cancellationToken">Token to handle request cancellation.</param>
  /// <returns>A unit value indicating successful execution.</returns>
  async Task<Unit> IRequestHandler<UpdateGateCommand, Unit>.Handle(UpdateGateCommand request,
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
          Value = request.UpdateGateDTO.Id.ToString()
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

    // Apply the updates from the DTO
    mapper.Map(request.UpdateGateDTO, gate);

    // Replace the existing entity with updated data
    gateUnitOfWork.Gates.ReplaceOne(gate);

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
