namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Core.Queries;

/// <summary>
/// Represents a base query for retrieving paginated and filtered collections of type <typeparamref name="TResponse"/>.
/// </summary>
/// <param name="Parameters">The request parameters containing filtering and pagination logic.</param>
public abstract record FindEntitiesByFilterQuery<TResponse>(RequestParameters Parameters)
    : IQuery<PagedList<TResponse>>;

/// <summary>
/// Provides a base handler for executing filtered and paginated queries.
/// Converts entities of type <typeparamref name="TEntity"/> to DTOs of type <typeparamref name="TDto"/>.
/// </summary>
/// <typeparam name="TEntity">The domain entity type, derived from <see cref="BaseEntity"/>.</typeparam>
/// <typeparam name="TDto">The target DTO type.</typeparam>
/// <typeparam name="TQuery">The query type, inheriting from <see cref="FindEntitiesByFilterQuery{TResponse}"/>.</typeparam>
/// <param name="specification">A reusable specification object for constructing filters.</param>
/// <param name="queryFilterParser">Parses textual filters into executable expressions.</param>
/// <param name="mapper">Maps entities to their DTO equivalents.</param>
internal abstract class FindEntitiesByFilterQueryHandler<TEntity, TDto, TQuery>(ISpecification<TEntity> specification,
                                                                                IQueryFilterParser queryFilterParser,
                                                                                IMapper mapper)
  : IQueryHandler<TQuery, PagedList<TDto>>
  where TEntity : BaseEntity
  where TDto : class
  where TQuery : FindEntitiesByFilterQuery<TDto>
{
  /// <summary>
  /// Handles the query request by applying filters, pagination, and mapping logic.
  /// </summary>
  /// <param name="request">The incoming query instance.</param>
  /// <param name="cancellationToken">Used to propagate cancellation signals.</param>
  /// <returns>A paginated list of mapped DTOs matching the query criteria.</returns>
  async Task<PagedList<TDto>> IRequestHandler<TQuery, PagedList<TDto>>.Handle(TQuery request, CancellationToken cancellationToken)
  {
    var specification = BuildSpecification(request.Parameters);
    var pagination = BuildPagination(request.Parameters);

    if (request.Parameters.IsPaginated)
      specification.ConfigurePagination(pagination);

    var entities = await FetchEntitiesAsync(specification, cancellationToken);

    if (IsEmptyResult(entities))
      return PagedList<TDto>.Empty();

    var totalCount = await CountResultsAsync(specification, cancellationToken);
    var dtos = MapToDtos(entities);

    return PagedList<TDto>.Create(dtos, totalCount, pagination);
  }

  /// <summary>
  /// Constructs a filtering specification based on request parameters.
  /// </summary>
  /// <param name="requestParameters">The request parameters containing filter expressions.</param>
  /// <returns>A specification for filtering <typeparamref name="TEntity"/> entities.</returns>
  protected virtual ISpecification<TEntity> BuildSpecification(RequestParameters requestParameters)
  {
    var filterExpr = queryFilterParser.ParseFilters<TEntity>(requestParameters.Filters);
    var spec = specification.Clone();

    if (filterExpr is not null)
      spec.AddFilter(filterExpr);

    return spec;
  }

  /// <summary>
  /// Creates a pagination context based on the request parameters.
  /// </summary>
  /// <param name="parameters">Contains page number and page size settings.</param>
  /// <returns>A configured <see cref="PaginationContext"/> object.</returns>
  protected static PaginationContext BuildPagination(RequestParameters parameters)
    => new(parameters.PageNumber ?? 0, parameters.PageSize ?? 0);

  /// <summary>
  /// Determines if the provided entity collection is empty.
  /// </summary>
  /// <param name="entities">Collection of queried entities.</param>
  /// <returns><c>true</c> if the result is null or empty; otherwise, <c>false</c>.</returns>
  protected static bool IsEmptyResult(IEnumerable<TEntity> entities)
    => entities == null || !entities.Any();

  /// <summary>
  /// Retrieves entities based on the provided specification.
  /// </summary>
  /// <param name="specification">Specification containing filters and pagination logic.</param>
  /// <param name="cancellationToken">Signals if the task should be cancelled.</param>
  /// <returns>A collection of <typeparamref name="TEntity"/> instances matching the specification.</returns>
  protected abstract Task<IEnumerable<TEntity>> FetchEntitiesAsync(ISpecification<TEntity> specification,
                                                                   CancellationToken cancellationToken);

  /// <summary>
  /// Counts the total number of entities that match the provided specification.
  /// </summary>
  /// <param name="specification">Specification used for counting entities.</param>
  /// <param name="cancellationToken">Cancellation token for interruption support.</param>
  /// <returns>The total count of matching entities.</returns>
  protected abstract Task<int> CountResultsAsync(ISpecification<TEntity> specification,
                                                 CancellationToken cancellationToken);

  /// <summary>
  /// Maps a collection of domain entities to DTOs.
  /// </summary>
  /// <param name="entities">The source entities to convert.</param>
  /// <returns>A collection of <typeparamref name="TDto"/> representing the transformed output.</returns>
  protected virtual IEnumerable<TDto> MapToDtos(IEnumerable<TEntity> entities)
    => mapper.Map<IEnumerable<TDto>>(entities);
}
