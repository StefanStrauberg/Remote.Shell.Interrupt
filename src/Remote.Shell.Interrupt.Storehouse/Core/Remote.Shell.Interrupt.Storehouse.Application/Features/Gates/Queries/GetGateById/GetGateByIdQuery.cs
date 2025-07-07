namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Queries.GetGateById;

/// <summary>
/// Defines a query for retrieving a gate by its unique identifier.
/// </summary>
public record GetGateByIdQuery(Guid Id) : IQuery<GateDTO>;

/// <summary>
/// Handles <see cref="GetGateByIdQuery"/> by validating existence,
/// applying filtering, and returning the mapped gate DTO.
/// </summary>
/// <param name="gateUnitOfWork">Provides access to gate-related data operations.</param>
/// <param name="baseSpecification">Clonable base specification used to build query filters.</param>
/// <param name="queryFilterParser">Parses textual filters into executable query expressions.</param>
/// <param name="mapper">Transforms gate entities into <see cref="GateDTO"/> objects.</param>
internal class GetGateByIdQueryHandler(IGateUnitOfWork gateUnitOfWork,
                                       IGateSpecification baseSpecification,
                                       IQueryFilterParser queryFilterParser,
                                       IMapper mapper)
  : IQueryHandler<GetGateByIdQuery, GateDTO>
{
  /// <summary>
  /// Executes the query by applying filters, checking existence, and retrieving the gate entity.
  /// </summary>
  /// <param name="request">Contains the gate ID to locate.</param>
  /// <param name="cancellationToken">Signals cancellation if requested by the caller.</param>
  /// <returns>A mapped <see cref="GateDTO"/> corresponding to the specified ID.</returns>
  async Task<GateDTO> IRequestHandler<GetGateByIdQuery, GateDTO>.Handle(GetGateByIdQuery request,
                                                                        CancellationToken cancellationToken)
  {
    var requestParameters = RequestParameters.ForId(request.Id);
    var filter = BuildFilteringSpec(requestParameters);

    var isExist = await CheckIfGateExistsAsync(filter, cancellationToken);

    if (isExist is not true)
      ThrowNotFoundException(requestParameters);

    var gate = await FetchGateAsync(filter, cancellationToken);

    return MapToDto(gate);
  }

  /// <summary>
  /// Throws an <see cref="EntityNotFoundException"/> if the gate is not found using provided parameters.
  /// </summary>
  /// <param name="parameters">Filtering parameters used for gate lookup.</param>
  void ThrowNotFoundException(RequestParameters parameters)
  {
    var filterExpr = queryFilterParser.ParseFilters<Gate>(parameters.Filters);
    throw new EntityNotFoundException(typeof(Gate), filterExpr is not null ? filterExpr.ToString() : string.Empty);
  }

  /// <summary>
  /// Determines whether a gate exists that matches the specified filter.
  /// </summary>
  /// <param name="filter">Filtering specification for gate lookup.</param>
  /// <param name="cancellationToken">Token for cancellation support.</param>
  /// <returns><c>true</c> if a matching gate is found; otherwise, <c>false</c>.</returns>
  async Task<bool> CheckIfGateExistsAsync(ISpecification<Gate> filter, CancellationToken cancellationToken)
    => await gateUnitOfWork.Gates.AnyByQueryAsync(filter, cancellationToken);

  /// <summary>
  /// Builds a filtering specification based on the provided request parameters.
  /// </summary>
  /// <param name="parameters">Request filters to apply during gate query.</param>
  /// <returns>A specification object containing filtering logic.</returns>
  ISpecification<Gate> BuildFilteringSpec(RequestParameters parameters)
  {
    var filterExpr = queryFilterParser.ParseFilters<Gate>(parameters.Filters);
    var spec = baseSpecification.Clone();

    if (filterExpr is not null)
      spec.AddFilter(filterExpr);

    return spec;
  }

  /// <summary>
  /// Fetches a gate entity matching the filtering specification.
  /// </summary>
  /// <param name="spec">Filtering specification used during query.</param>
  /// <param name="cancellationToken">Token indicating cancellation request.</param>
  /// <returns>The retrieved <see cref="Gate"/> entity.</returns>
  async Task<Gate> FetchGateAsync(ISpecification<Gate> spec, CancellationToken cancellationToken)
    => await gateUnitOfWork.Gates.GetOneShortAsync(spec, cancellationToken);

  /// <summary>
  /// Maps a gate domain entity to its data transfer object equivalent.
  /// </summary>
  /// <param name="entities">Gate entity to convert.</param>
  /// <returns>A mapped <see cref="GateDTO"/> instance.</returns>
  GateDTO MapToDto(Gate entities)
    => mapper.Map<GateDTO>(entities);
}
