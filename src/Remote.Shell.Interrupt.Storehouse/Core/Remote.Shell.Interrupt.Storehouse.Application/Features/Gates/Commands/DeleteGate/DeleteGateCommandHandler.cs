namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.DeleteGate;

/// <summary>
/// Defines a command to delete a gate identified by its unique <c>Id</c>.
/// </summary>
/// <param name="Id">The unique identifier of the gate to be deleted.</param>
public record DeleteGateCommand(Guid Id) : ICommand<Unit>;

/// <summary>
/// Handles <see cref="DeleteGateCommand"/> by validating gate existence,
/// retrieving the entity, and performing a deletion transaction.
/// </summary>
/// <param name="gateUnitOfWork">Provides repository access and transactional control for gate entities.</param>
/// <param name="baseSpecification">Base specification used for query construction.</param>
/// <param name="queryFilterParser">Parses textual filters into expression-based predicates.</param>
internal class DeleteGateCommandHandler(IGateUnitOfWork gateUnitOfWork,
                                        IGateSpecification baseSpecification,
                                        IQueryFilterParser queryFilterParser)
: ICommandHandler<DeleteGateCommand, Unit>
{
  /// <summary>
  /// Executes gate deletion logic based on the ID specified in the request.
  /// </summary>
  /// <param name="request">Command containing the ID of the gate to delete.</param>
  /// <param name="cancellationToken">Propagates cancellation if requested.</param>
  /// <returns><see cref="Unit.Value"/> upon successful deletion.</returns>
  async Task<Unit> IRequestHandler<DeleteGateCommand, Unit>.Handle(DeleteGateCommand request,
                                                                   CancellationToken cancellationToken)
  {
    var specification = BuildSpecification(request.Id);

    await EnsureGateExistAsync(specification, cancellationToken);
    var gate = await FetchGateAsync(specification, cancellationToken);

    DeleteGate(gate);

    return Unit.Value;
  }

  /// <summary>
  /// Builds a specification for querying the gate using the provided ID.
  /// </summary>
  /// <param name="gateId">The unique identifier used for gate filtering.</param>
  /// <returns>A specification object configured with the ID filter.</returns>
  ISpecification<Gate> BuildSpecification(Guid gateId)
  {
    var filterExpr = queryFilterParser.ParseFilters<Gate>(RequestParameters.ForId(gateId).Filters);
    var spec = baseSpecification.Clone();

    if (filterExpr is not null)
      spec.AddFilter(filterExpr);

    return spec;
  }

  /// <summary>
  /// Verifies that a gate matching the given specification exists.
  /// </summary>
  /// <param name="specification">Specification used to locate the gate.</param>
  /// <param name="cancellationToken">Used to monitor cancellation requests.</param>
  /// <exception cref="EntityAlreadyExists">Thrown if no gate is found for deletion.</exception>
  async Task EnsureGateExistAsync(ISpecification<Gate> specification,
                                  CancellationToken cancellationToken)
  {
    bool exists = await gateUnitOfWork.Gates.AnyByQueryAsync(specification, cancellationToken);

    if (exists is not true)
      throw new EntityAlreadyExists(typeof(Gate), specification.ToString() ?? string.Empty);
  }

  /// <summary>
  /// Retrieves the gate entity to be deleted.
  /// </summary>
  /// <param name="spec">Specification used to identify the gate.</param>
  /// <param name="cancellationToken">Token used to observe cancellation requests.</param>
  /// <returns>The gate entity targeted for deletion.</returns>
  async Task<Gate> FetchGateAsync(ISpecification<Gate> spec, CancellationToken cancellationToken)
    => await gateUnitOfWork.Gates.GetOneShortAsync(spec, cancellationToken);

  /// <summary>
  /// Deletes the specified gate entity and finalizes the transaction.
  /// </summary>
  /// <param name="gate">The gate entity to delete.</param>
  void DeleteGate(Gate gate)
  {
    gateUnitOfWork.Gates.DeleteOne(gate);
    gateUnitOfWork.Complete();
  }
}
