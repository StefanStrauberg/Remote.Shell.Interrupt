namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Core.Commands;

public abstract record UpdateEntityCommand<TEntity, TDto>(Guid EntityId, TDto DtoEntity) 
  : ICommand<Unit>;

abstract class UpdateEntityCommandHandler<TEntity, TDto, TCommand>(ISpecification<TEntity> specification,
                                                                   IQueryFilterParser queryFilterParser,
                                                                   IMapper mapper)
  : ICommandHandler<TCommand, Unit>
  where TEntity : BaseEntity
  where TDto : class
  where TCommand : UpdateEntityCommand<TEntity, TDto>
{
  async Task<Unit> IRequestHandler<TCommand, Unit>.Handle(TCommand request, CancellationToken cancellationToken)
  {
    var specification = BuildSpecification(request.EntityId);

    await EnsureEntityExistAsync(specification, cancellationToken);

    var entity = await FetchEntityAsync(specification, cancellationToken);

    MapToDto(request.DtoEntity, entity);

    UpdateEntity(entity);

    return Unit.Value;
  }

  protected virtual ISpecification<TEntity> BuildSpecification(Guid entityId)
  {
    var filterExpr = queryFilterParser.ParseFilters<TEntity>(RequestParameters.ForId(entityId).Filters);
    var spec = specification.Clone();

    if (filterExpr is not null)
      spec.AddFilter(filterExpr);

    return spec;
  }

  protected abstract Task EnsureEntityExistAsync(ISpecification<TEntity> specification,
                                                 CancellationToken cancellationToken);

  protected abstract Task<TEntity> FetchEntityAsync(ISpecification<TEntity> specification,
                                                    CancellationToken cancellationToken);

  protected virtual void MapToDto(TDto source, TEntity destination)
    => mapper.Map(source, destination);

  protected abstract void UpdateEntity(TEntity entity);
}
