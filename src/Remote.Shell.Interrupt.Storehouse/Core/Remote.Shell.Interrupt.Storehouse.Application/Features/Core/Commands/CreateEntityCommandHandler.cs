namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Core.Commands;

/// <summary>
/// Represents a generic command for creating a new entity using a DTO payload.
/// </summary>
/// <typeparam name="TCreateDto">The type of DTO used to populate the entity.</typeparam>
public abstract record CreateEntityCommand<TCreateDto>(TCreateDto CreateDto)
  : ICommand<Unit>;

/// <summary>
/// Base handler for processing <see cref="CreateEntityCommand{TCreateDto}"/> instances.
/// Includes validation, mapping, and persistence steps with support for specification-based duplicate prevention.
/// </summary>
/// <typeparam name="TEntity">The type of the entity being created.</typeparam>
/// <typeparam name="TDto">The type of DTO carrying creation data.</typeparam>
/// <typeparam name="TCommand">The specific command type inheriting from <see cref="CreateEntityCommand{TDto}"/>.</typeparam>
internal abstract class CreateEntityCommandHandler<TEntity, TDto, TCommand>(ISpecification<TEntity> specification,
                                                                            IMapper mapper)
  : ICommandHandler<TCommand, Unit>
  where TEntity : BaseEntity
  where TDto : class
  where TCommand : CreateEntityCommand<TDto>
{
  async Task<Unit> IRequestHandler<TCommand, Unit>.Handle(TCommand request, CancellationToken cancellationToken)
  {
    var filter = BuildDuplicateCheckFilter(request.CreateDto);
    var specification = BuildSpecification(filter);

    await ValidateEntityDoesNotExistAsync(specification, cancellationToken);

    var newEntity = MapToEntity(request.CreateDto);
    PersistNewEntity(newEntity);

    return Unit.Value;
  }

  /// <summary>
  /// Builds a filter expression used to determine entity uniqueness prior to creation.
  /// </summary>
  /// <param name="dto">The DTO representing the creation payload.</param>
  /// <returns>A LINQ expression for entity filtering, or <c>null</c> if no filter is needed.</returns>
  protected abstract Expression<Func<TEntity, bool>>? BuildDuplicateCheckFilter(TDto dto);

  /// <summary>
  /// Applies the base specification and adds optional filter logic if provided.
  /// </summary>
  /// <param name="filterExpr">The filter expression to layer on top of the specification.</param>
  /// <returns>The constructed specification to query for duplicates.</returns>
  protected virtual ISpecification<TEntity> BuildSpecification(Expression<Func<TEntity, bool>>? filterExpr)
  {
    var spec = specification.Clone();

    if (filterExpr is not null)
      spec.AddFilter(filterExpr);

    return spec;
  }

  /// <summary>
  /// Validates that no existing entity matches the given specification criteria.
  /// Throws if a duplicate is detected.
  /// </summary>
  /// <param name="specification">The specification used to identify duplicates.</param>
  /// <param name="cancellationToken">Token used to cancel the operation.</param>
  protected abstract Task ValidateEntityDoesNotExistAsync(ISpecification<TEntity> specification,
                                                          CancellationToken cancellationToken);

  /// <summary>
  /// Maps the DTO to a new entity instance.
  /// </summary>
  /// <param name="dto">The input DTO used for creation.</param>
  /// <returns>A newly constructed entity.</returns>
  TEntity MapToEntity(TDto dto)
    => mapper.Map<TEntity>(dto);

  /// <summary>
  /// Persists the newly created entity to the underlying data store.
  /// </summary>
  /// <param name="entity">The entity to save.</param>
  protected abstract void PersistNewEntity(TEntity entity);
}
