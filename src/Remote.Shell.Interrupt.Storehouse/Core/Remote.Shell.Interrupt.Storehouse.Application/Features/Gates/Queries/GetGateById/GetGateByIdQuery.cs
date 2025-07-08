namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Queries.GetGateById;

/// <summary>
/// Represents a query to retrieve a single <see cref="GateDTO"/> by its unique identifier.
/// </summary>
/// <param name="Id">The unique ID used to locate the gate entity.</param>
public sealed record GetGateByIdQuery(Guid Id)
  : FindEntityByIdQuery<GateDTO>(Id);

/// <summary>
/// Handles <see cref="GetGateByIdQuery"/> by validating the existence of the gate entity,
/// retrieving it from the data store, and mapping it to a DTO.
/// </summary>
/// <param name="gateUnitOfWork">Provides access to gate-related persistence operations.</param>
/// <param name="specification">A clonable specification used to build filter criteria.</param>
/// <param name="queryFilterParser">Parses filter definitions into expression trees.</param>
/// <param name="mapper">Maps <see cref="Gate"/> entities to <see cref="GateDTO"/> representations.</param>
internal class GetGateByIdQueryHandler(IGateUnitOfWork gateUnitOfWork,
                                       IGateSpecification specification,
                                       IQueryFilterParser queryFilterParser,
                                       IMapper mapper)
  : FindEntityByIdQueryHandler<Gate, GateDTO, GetGateByIdQuery>(specification, queryFilterParser, mapper)
{
  /// <summary>
  /// Ensures that a gate entity matching the given specification exists; otherwise, throws an exception.
  /// </summary>
  /// <param name="specification">The specification defining the gate lookup criteria.</param>
  /// <param name="cancellationToken">Token used to signal cancellation.</param>
  /// <exception cref="EntityAlreadyExists">Thrown when no gate is found matching the specification.</exception>
  protected override async Task EnsureEntityExistAsync(ISpecification<Gate> specification,
                                                       CancellationToken cancellationToken)
  {
    bool exists = await gateUnitOfWork.Gates.AnyByQueryAsync(specification, cancellationToken);

    if (exists is not true)
      throw new EntityAlreadyExists(typeof(Gate), specification.ToString() ?? string.Empty);
  }

  /// <summary>
  /// Retrieves the gate entity that matches the provided filter specification.
  /// </summary>
  /// <param name="spec">The specification used to locate the gate.</param>
  /// <param name="cancellationToken">Token used to monitor for cancellation.</param>
  /// <returns>The matching <see cref="Gate"/> entity.</returns>
  protected override async Task<Gate> FetchEntityAsync(ISpecification<Gate> spec,
                                                       CancellationToken cancellationToken)
    => await gateUnitOfWork.Gates.GetOneShortAsync(spec, cancellationToken);
}
