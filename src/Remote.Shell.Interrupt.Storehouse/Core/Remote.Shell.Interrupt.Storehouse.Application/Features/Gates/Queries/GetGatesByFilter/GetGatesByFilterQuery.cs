namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Queries.GetGatesByFilter;

/// <summary>
/// Represents a query to retrieve Gate entities based on filter and pagination criteria.
/// </summary>
/// <param name="Parameters">The request parameters containing filtering and pagination data.</param>
public record GetGatesByFilterQuery(RequestParameters Parameters) 
  : IQuery<PagedList<GateDTO>>;

/// <summary>
/// Handles the <see cref="GetGatesByFilterQuery"/> by applying filtering,
/// pagination, and mapping logic to return a paged list of gate DTOs.
/// </summary>
/// <param name="gateUnitOfWork">The unit of work providing access to gate-related repositories.</param>
/// <param name="baseSpecification">The base specification to build filtered queries upon.</param>
/// <param name="queryFilterParser">Parser used to convert filter strings to LINQ expressions.</param>
/// <param name="mapper">Mapper used to convert domain entities to DTOs.</param>
internal class GetGatesByFilterQueryHandler(IGateUnitOfWork gateUnitOfWork,
                                            IGateSpecification baseSpecification,
                                            IQueryFilterParser queryFilterParser,
                                            IMapper mapper)
  : IQueryHandler<GetGatesByFilterQuery, PagedList<GateDTO>>
{
  /// <summary>
  /// Executes the query request by applying filters, retrieving gate entities,
  /// and returning a paged result of mapped DTOs.
  /// </summary>
  /// <param name="request">The query request containing filtering and pagination settings.</param>
  /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
  /// <returns>A paginated list of <see cref="GateDTO"/> objects matching the criteria.</returns>
  async Task<PagedList<GateDTO>> IRequestHandler<GetGatesByFilterQuery, PagedList<GateDTO>>.Handle(GetGatesByFilterQuery request,
                                                                                                   CancellationToken cancellationToken)
  {
    var specification = BuildSpecification(request.Parameters);
    var pagination = BuildPagination(request.Parameters);

    if (request.Parameters.IsPaginated)
      specification.ConfigurePagination(pagination);

    var gates = await FetchGatesAsync(specification, cancellationToken);

    if (IsEmptyResult(gates))
      return PagedList<GateDTO>.Empty();

    var totalCount = await CountGatesAsync(specification, cancellationToken);
    var gateDtos = MapToGateDtos(gates);

    return PagedList<GateDTO>.Create(gateDtos, totalCount, pagination);
  }

  /// <summary>
  /// Builds a filtering specification for querying gates based on provided parameters.
  /// </summary>
  /// <param name="parameters">The request parameters containing optional filtering expressions.</param>
  /// <returns>An <see cref="ISpecification{T}"/> representing the applied filter criteria.</returns>
  ISpecification<Gate> BuildSpecification(RequestParameters parameters)
  {
    var filterExpr = queryFilterParser.ParseFilters<Gate>(parameters.Filters);
    var spec = baseSpecification.Clone();

    if (filterExpr is not null)
      spec.AddFilter(filterExpr);

    return spec;
  }

  /// <summary>
  /// Generates a <see cref="PaginationContext"/> using pagination values from the request.
  /// </summary>
  /// <param name="parameters">The request parameters specifying pagination options.</param>
  /// <returns>A pagination context including page number and size, defaulting to 0 if unspecified.</returns>
  static PaginationContext BuildPagination(RequestParameters parameters)
    => new(parameters.PageNumber ?? 0, parameters.PageSize ?? 0);

  /// <summary>
  /// Retrieves a filtered collection of <see cref="Gate"/> entities asynchronously.
  /// </summary>
  /// <param name="spec">The filter specification to apply during retrieval.</param>
  /// <param name="cancellationToken">Token to observe for cancellation signals.</param>
  /// <returns>A collection of gates matching the filter criteria.</returns>
  async Task<IEnumerable<Gate>> FetchGatesAsync(ISpecification<Gate> spec, CancellationToken cancellationToken)
    => await gateUnitOfWork.Gates.GetManyShortAsync(spec, cancellationToken);

  /// <summary>
  /// Retrieves the total number of gate records matching the provided specification.
  /// </summary>
  /// <param name="spec">Specification used to count matching records.</param>
  /// <param name="cancellationToken">Token for cancellation support.</param>
  /// <returns>The total number of matching gate records.</returns>
  async Task<int> CountGatesAsync(ISpecification<Gate> spec, CancellationToken cancellationToken)
    => await gateUnitOfWork.Gates.GetCountAsync(spec, cancellationToken);

  /// <summary>
  /// Maps gate domain entities to their corresponding data transfer objects.
  /// </summary>
  /// <param name="entities">The gate entities to map.</param>
  /// <returns>A collection of <see cref="GateDTO"/> instances.</returns>
  IEnumerable<GateDTO> MapToGateDtos(IEnumerable<Gate> entities)
    => mapper.Map<IEnumerable<GateDTO>>(entities);

  /// <summary>
  /// Checks if the provided collection of gates is null or empty.
  /// </summary>
  /// <param name="gates">The collection to evaluate.</param>
  /// <returns><c>true</c> if the collection is null or contains no elements; otherwise, <c>false</c>.</returns>
  static bool IsEmptyResult(IEnumerable<Gate> gates)
    => gates == null || !gates.Any();
}
