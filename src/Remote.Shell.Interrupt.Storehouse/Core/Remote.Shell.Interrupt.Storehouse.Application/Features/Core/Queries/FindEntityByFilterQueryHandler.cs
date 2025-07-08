namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Core.Queries;

/// <summary>
/// Represents a query to retrieve a single entity of type <typeparamref name="TResponse"/> 
/// based on filter criteria provided in <see cref="RequestParameters"/>.
/// </summary>
/// <param name="Parameters">Contains filtering rules used to locate the entity.</param>
public abstract record FindEntityByFilterQuery<TResponse>(RequestParameters Parameters)
  : IRequest<TResponse>;

/// <summary>
/// Provides an abstract base for handling queries that retrieve a single entity via filtering criteria.
/// It builds a specification from filters, validates existence, and returns a mapped result.
/// </summary>
/// <typeparam name="TEntity">The domain entity type, inheriting from <see cref="BaseEntity"/>.</typeparam>
/// <typeparam name="TDto">The data transfer object type returned by the handler.</typeparam>
/// <typeparam name="TQuery">The concrete query type, derived from <see cref="FindEntityByFilterQuery{TResponse}"/>.</typeparam>
/// <param name="specification">Base specification used to construct filter logic.</param>
/// <param name="queryFilterParser">Parses filter descriptors into expression trees.</param>
/// <param name="mapper">Maps domain entities to DTO representations.</param>
internal abstract class FindEntityByFilterQueryHandler<TEntity, TDto, TQuery>(ISpecification<TEntity> specification,
                                                                              IQueryFilterParser queryFilterParser,
                                                                              IMapper mapper)
  : IRequestHandler<TQuery, TDto>
  where TEntity : BaseEntity
  where TDto : class
  where TQuery : FindEntityByFilterQuery<TDto>
{
  /// <summary>
  /// Executes the query: builds a filtering specification, validates entity existence,
  /// fetches the entity, and maps it to its DTO.
  /// </summary>
  /// <param name="request">The query containing filter parameters.</param>
  /// <param name="cancellationToken">Used to propagate cancellation signals.</param>
  /// <returns>A mapped <typeparamref name="TDto"/> object representing the result.</returns>
  async Task<TDto> IRequestHandler<TQuery, TDto>.Handle(TQuery request, CancellationToken cancellationToken)
  {
    var specification = BuildSpecification(request.Parameters);

    await EnsureEntityExistAsync(specification, cancellationToken);

    var entity = await FetchEntityAsync(specification, cancellationToken);

    return MapToDto(entity);
  }

  /// <summary>
  /// Validates that an entity exists for the given specification.
  /// </summary>
  /// <param name="specification">Specification used to query the entity.</param>
  /// <param name="cancellationToken">Token for canceling the operation if needed.</param>
  protected abstract Task EnsureEntityExistAsync(ISpecification<TEntity> specification,
                                                 CancellationToken cancellationToken);

  /// <summary>
  /// Builds a filtering specification using the request parameters.
  /// </summary>
  /// <param name="requestParameters">Contains filter expressions to be applied.</param>
  /// <returns>A specification with the relevant filtering logic.</returns>
  protected virtual ISpecification<TEntity> BuildSpecification(RequestParameters requestParameters)
  {
    var filterExpr = queryFilterParser.ParseFilters<TEntity>(requestParameters.Filters);
    var spec = specification.Clone();

    if (filterExpr is not null)
      spec.AddFilter(filterExpr);

    return spec;
  }

  /// <summary>
  /// Retrieves the entity matching the given specification.
  /// </summary>
  /// <param name="specification">Specification used to locate the entity.</param>
  /// <param name="cancellationToken">Supports cancellation for the operation.</param>
  /// <returns>The retrieved <typeparamref name="TEntity"/> instance.</returns>
  protected abstract Task<TEntity> FetchEntityAsync(ISpecification<TEntity> specification,
                                                    CancellationToken cancellationToken);

  /// <summary>
  /// Maps a domain entity to its corresponding DTO.
  /// </summary>
  /// <param name="entity">The entity to transform.</param>
  /// <returns>The mapped <typeparamref name="TDto"/>.</returns>
  protected virtual TDto MapToDto(TEntity entity)
    => mapper.Map<TDto>(entity);
}
