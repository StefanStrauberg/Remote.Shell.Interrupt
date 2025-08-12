namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.DeleteGate;

/// <summary>
/// Command to delete a <see cref="Gate"/> entity by its unique identifier.
/// </summary>
public record DeleteGateCommand(Guid Id) : DeleteEntityCommand(Id);

/// <summary>
/// Handler for processing <see cref="DeleteGateCommand"/> instances.
/// Implements gate-specific logic for existence validation, retrieval, and deletion.
/// </summary>
/// <param name="gateUnitOfWork">Unit of work for gate-related operations.</param>
/// <param name="specification">Base specification used for querying gates.</param>
/// <param name="queryFilterParser">Parser for translating request filters into query expressions.</param>
internal class DeleteGateCommandHandler(IGateUnitOfWork gateUnitOfWork,
                                        IGateSpecification specification,
                                        IQueryFilterParser queryFilterParser)
: DeleteEntityCommandHandler<Gate, DeleteGateCommand>(specification, queryFilterParser)
{
  /// <summary>
  /// Verifies that a gate entity matching the specification exists.
  /// Throws <see cref="EntityAlreadyExists"/> if no match is found.
  /// </summary>
  /// <param name="specification">Specification used to locate the gate.</param>
  /// <param name="cancellationToken">Token for cancelling the operation.</param>
  /// <exception cref="EntityAlreadyExists">
  /// Thrown when no gate matches the provided specification.
  /// </exception>
  protected override async Task EnsureEntityExistAsync(ISpecification<Gate> specification,
                                                       CancellationToken cancellationToken)
  {
    bool exists = await gateUnitOfWork.Gates.AnyByQueryAsync(specification, cancellationToken);

    if (exists is not true)
      throw new EntityAlreadyExists(typeof(Gate), specification.ToString() ?? string.Empty);
  }

  /// <summary>
  /// Retrieves the gate entity to be deleted using the provided specification.
  /// </summary>
  /// <param name="specification">Specification used to locate the gate.</param>
  /// <param name="cancellationToken">Token for cancelling the operation.</param>
  /// <returns>The gate entity to delete.</returns>
  protected override async Task<Gate> FetchEntityAsync(ISpecification<Gate> specification, CancellationToken cancellationToken)
    => await gateUnitOfWork.Gates.GetOneShortAsync(specification, cancellationToken);

  /// <summary>
  /// Deletes the specified gate entity and commits the unit of work.
  /// </summary>
  /// <param name="entity">The gate entity to delete.</param>
  protected override void DeleteEntity(Gate entity)
  {
    gateUnitOfWork.Gates.DeleteOne(entity);
    gateUnitOfWork.Complete();
  }
}
