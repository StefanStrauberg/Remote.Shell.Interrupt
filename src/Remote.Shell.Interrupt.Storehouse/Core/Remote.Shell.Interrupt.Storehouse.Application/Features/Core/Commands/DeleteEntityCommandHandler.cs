namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Core.Commands;

/// <summary>
/// Represents a command to delete an entity by its unique identifier.
/// </summary>
public abstract record DeleteEntityCommand(Guid Id)
  : ICommand<Unit>;

/// <summary>
/// Abstract base handler for processing <see cref="DeleteEntityCommand"/> instances.
/// Encapsulates specification-based entity lookup, existence validation, and deletion logic.
/// </summary>
/// <typeparam name="TEntity">The type of entity being deleted.</typeparam>
/// <typeparam name="TCommand">The specific command type inheriting from <see cref="DeleteEntityCommand"/>.</typeparam>
internal abstract class DeleteEntityCommandHandler<TEntity, TCommand>(ISpecification<TEntity> specification,
                                                                      IQueryFilterParser queryFilterParser)
  : IRequestHandler<TCommand, Unit>
  where TEntity : BaseEntity
  where TCommand : DeleteEntityCommand
{
  /// <summary>
  /// Handles the delete command by verifying existence, fetching the entity, and invoking deletion.
  /// </summary>
  /// <param name="request">The delete command containing the entity ID.</param>
  /// <param name="cancellationToken">Token for cancelling the operation.</param>
  /// <returns>A <see cref="Unit"/> result indicating completion.</returns>
  async Task<Unit> IRequestHandler<TCommand, Unit>.Handle(TCommand request, CancellationToken cancellationToken)
  {
    var specification = BuildSpecification(request.Id);

    await EnsureEntityExistAsync(specification, cancellationToken);

    var entity = await FetchEntityAsync(specification, cancellationToken);

    DeleteEntity(entity);

    return Unit.Value;
  }

  /// <summary>
  /// Validates that the entity exists before attempting deletion.
  /// </summary>
  /// <param name="specification">The specification used to locate the entity.</param>
  /// <param name="cancellationToken">Token for cancelling the operation.</param>
  /// <returns>A task representing the verification operation.</returns>
  protected abstract Task EnsureEntityExistAsync(ISpecification<TEntity> specification,
                                                 CancellationToken cancellationToken);

  /// <summary>
  /// Builds a specification for locating the entity by ID.
  /// </summary>
  /// <param name="entityId">The unique identifier of the entity.</param>
  /// <returns>A specification with ID-based filtering applied.</returns>
  /// <remarks>
  /// Clones the base specification and injects a filter parsed from <see cref="RequestParametersFactory.ForId"/>.
  /// This allows for flexible query composition while preserving immutability.
  /// </remarks>
  protected virtual ISpecification<TEntity> BuildSpecification(Guid entityId)
  {
    var filterExpr = queryFilterParser.ParseFilters<TEntity>(RequestParametersFactory.ForId(entityId).Filters);
    var spec = specification.Clone();

    if (filterExpr is not null)
      spec.AddFilter(filterExpr);

    return spec;
  }

  /// <summary>
  /// Fetches the entity to be deleted using the provided specification.
  /// </summary>
  /// <param name="specification">The specification used to locate the entity.</param>
  /// <param name="cancellationToken">Token for cancelling the operation.</param>
  /// <returns>The entity to be deleted.</returns>
  protected abstract Task<TEntity> FetchEntityAsync(ISpecification<TEntity> specification,
                                                    CancellationToken cancellationToken);

  /// <summary>
  /// Performs the actual deletion of the entity.
  /// </summary>
  /// <param name="entity">The entity to delete.</param>
  protected abstract void DeleteEntity(TEntity entity);
}
