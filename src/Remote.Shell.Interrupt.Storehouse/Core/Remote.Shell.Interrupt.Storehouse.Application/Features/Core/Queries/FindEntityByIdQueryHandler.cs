namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Core.Queries;

/// <summary>
/// Represents a query to retrieve a single entity of type <typeparamref name="TResponse"/> by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier used to locate the target entity.</param>
public abstract record FindEntityByIdQuery<TResponse>(Guid Id)
  : IRequest<TResponse>;

/// <summary>
/// Provides an abstract base for handling queries that retrieve a single entity by ID.
/// Converts a domain entity of type <typeparamref name="TEntity"/> to a response DTO of type <typeparamref name="TDto"/>.
/// </summary>
/// <typeparam name="TEntity">The domain entity type, inheriting from <see cref="BaseEntity"/>.</typeparam>
/// <typeparam name="TDto">The target data transfer object type.</typeparam>
/// <typeparam name="TQuery">The query type, which must inherit from <see cref="FindEntityByIdQuery{TResponse}"/>.</typeparam>
/// <param name="specification">The base specification used for building filter expressions.</param>
/// <param name="queryFilterParser">Parses structured filter definitions into executable expressions.</param>
/// <param name="mapper">Handles mapping between entity and DTO representations.</param>
internal abstract class FindEntityByIdQueryHandler<TEntity, TDto, TQuery>(ISpecification<TEntity> specification,
                                                                          IQueryFilterParser queryFilterParser,
                                                                          IMapper mapper)
  : IRequestHandler<TQuery, TDto>
  where TEntity : BaseEntity
  where TDto : class
  where TQuery : FindEntityByIdQuery<TDto>
{
  /// <summary>
  /// Executes the query by building a filtering specification,
  /// validating entity existence, fetching the entity, and mapping it to a DTO.
  /// </summary>
  /// <param name="request">The query containing the entity ID.</param>
  /// <param name="cancellationToken">Token used to observe cancellation requests.</param>
  /// <returns>The mapped <typeparamref name="TDto"/> if the entity is found.</returns>
  async Task<TDto> IRequestHandler<TQuery, TDto>.Handle(TQuery request, CancellationToken cancellationToken)
  {
    var specification = BuildSpecification(request.Id);

    await EnsureEntityExistAsync(specification, cancellationToken);

    var entity = await FetchEntityAsync(specification, cancellationToken);

    return MapToDto(entity);
  }

  /// <summary>
  /// Validates that an entity matching the specification exists.
  /// </summary>
  /// <param name="specification">Specification used to locate the target entity.</param>
  /// <param name="cancellationToken">Token used to monitor cancellation signals.</param>
  protected abstract Task EnsureEntityExistAsync(ISpecification<TEntity> specification,
                                                 CancellationToken cancellationToken);

  /// <summary>
  /// Builds a filtering specification for locating an entity by its unique ID.
  /// </summary>
  /// <param name="entityId">The unique identifier of the entity to retrieve.</param>
  /// <returns>A specification configured to locate the requested entity.</returns>
  protected virtual ISpecification<TEntity> BuildSpecification(Guid entityId)
  {
    var filterExpr = queryFilterParser.ParseFilters<TEntity>(RequestParameters.ForId(entityId).Filters);
    var spec = specification.Clone();

    if (filterExpr is not null)
      spec.AddFilter(filterExpr);

    return spec;
  }

  /// <summary>
  /// Retrieves the entity matching the given specification.
  /// </summary>
  /// <param name="specification">Filter specification used during lookup.</param>
  /// <param name="cancellationToken">Supports cancellation if requested.</param>
  /// <returns>The matching domain entity.</returns>
  protected abstract Task<TEntity> FetchEntityAsync(ISpecification<TEntity> specification,
                                                    CancellationToken cancellationToken);

  /// <summary>
  /// Maps a domain entity to its corresponding DTO.
  /// </summary>
  /// <param name="entity">The entity to be transformed.</param>
  /// <returns>The mapped <typeparamref name="TDto"/> object.</returns>
  protected virtual TDto MapToDto(TEntity entity)
    => mapper.Map<TDto>(entity);
}
