namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.DeleteGate;

/// <summary>
/// Represents a command to delete a gate by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the gate to delete.</param>
public record DeleteGateCommand(Guid Id) : ICommand<Unit>;

/// <summary>
/// Handles the DeleteGateCommand and removes the specified gate from the system.
/// </summary>
/// <remarks>
/// This handler verifies the existence of the gate, retrieves it, and performs the deletion.
/// If the gate does not exist, an exception is thrown.
/// </remarks>
/// <param name="gateUnitOfWork">Unit of work for gate-related database operations.</param>
/// <param name="specification">Specification used for gate filtering.</param>
/// <param name="queryFilterParser">Parser for processing filter expressions.</param>
internal class DeleteGateCommandHandler(IGateUnitOfWork gateUnitOfWork,
                                        IGateSpecification specification,
                                        IQueryFilterParser queryFilterParser)
: ICommandHandler<DeleteGateCommand, Unit>
{
  /// <summary>
  /// Handles the request to delete a gate by its ID.
  /// </summary>
  /// <param name="request">The command containing the gate ID to delete.</param>
  /// <param name="cancellationToken">Token to allow request cancellation.</param>
  /// <returns>A unit value indicating successful execution.</returns>
  async Task<Unit> IRequestHandler<DeleteGateCommand, Unit>.Handle(DeleteGateCommand request,
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

    // Delete the entity from the database
    gateUnitOfWork.Gates.DeleteOne(gate);

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
