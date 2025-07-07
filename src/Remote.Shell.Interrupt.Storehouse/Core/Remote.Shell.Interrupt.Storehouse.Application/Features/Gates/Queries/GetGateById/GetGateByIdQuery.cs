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
    var specification = BuildSpecification(request.Id);

    await EnsureGateExistAsync(specification, cancellationToken);

    var gate = await FetchGateAsync(specification, cancellationToken);

    return MapToDto(gate);
  }

  /// <summary>
  /// Validates that existing gate matches the provided specification.
  /// </summary>
  /// <param name="specification">Specification used for gate uniqueness check.</param>
  /// <param name="cancellationToken">Supports cancellation of the async operation.</param>
  /// <exception cref="EntityAlreadyExists">Thrown if a gate with the same filter already exists.</exception>
  async Task EnsureGateExistAsync(ISpecification<Gate> specification,
                                  CancellationToken cancellationToken)
  {
    bool exists = await gateUnitOfWork.Gates.AnyByQueryAsync(specification, cancellationToken);

    if (exists is not true)
      throw new EntityAlreadyExists(typeof(Gate), specification.ToString() ?? string.Empty);
  }

  /// <summary>
  /// Builds a specification for querying the gate using the provided ID.
  /// </summary>
  /// <param name="gateId">The unique identifier used for gate filtering.</param>
  /// <returns>A specification object containing filtering logic.</returns>
  ISpecification<Gate> BuildSpecification(Guid gateId)
  {
    var filterExpr = queryFilterParser.ParseFilters<Gate>(RequestParameters.ForId(gateId).Filters);
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
