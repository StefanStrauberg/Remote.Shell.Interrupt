namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Queries.GetGatesByFilter;

/// <summary>
/// Defines a query for retrieving a paginated and optionally filtered list of <see cref="GateDTO"/> items.
/// </summary>
/// <param name="Parameters">Contains filtering and pagination settings for the gate query.</param>
public sealed record GetGatesByFilterQuery(RequestParameters Parameters)
  : FindEntitiesByFilterQuery<GateDTO>(Parameters);

/// <summary>
/// Handles <see cref="GetGatesByFilterQuery"/> by executing filtering, pagination, and entity-to-DTO transformation.
/// </summary>
/// <param name="gateUnitOfWork">Provides repository access for gate-related data operations.</param>
/// <param name="specification">Base specification object used to compose query filters.</param>
/// <param name="queryFilterParser">Parses textual filters into LINQ-based expression logic.</param>
/// <param name="mapper">Maps domain <see cref="Gate"/> entities to their corresponding DTOs.</param>
internal class GetGatesByFilterQueryHandler(IGateUnitOfWork gateUnitOfWork,
                                            IGateSpecification specification,
                                            IQueryFilterParser queryFilterParser,
                                            IMapper mapper)
  : FindEntitiesByFilterQueryHandler<Gate, GateDTO, GetGatesByFilterQuery>(specification, queryFilterParser, mapper)

{
  /// <summary>
  /// Asynchronously retrieves gate entities matching the given specification.
  /// </summary>
  /// <param name="specification">Filtering specification used for gate retrieval.</param>
  /// <param name="cancellationToken">Token used to signal cancellation of the operation.</param>
  /// <returns>A collection of <see cref="Gate"/> entities that match the filtering conditions.</returns>
  protected override async Task<IEnumerable<Gate>> FetchEntitiesAsync(ISpecification<Gate> specification,
                                                                      CancellationToken cancellationToken)
    => await gateUnitOfWork.Gates.GetManyShortAsync(specification, cancellationToken);

  /// <summary>
  /// Asynchronously counts the total number of gate entities that match the provided specification.
  /// </summary>
  /// <param name="specification">Specification defining the filter logic for the count operation.</param>
  /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
  /// <returns>The number of <see cref="Gate"/> entities matching the filter.</returns>
  protected override async Task<int> CountResultsAsync(ISpecification<Gate> specification,
                                                       CancellationToken cancellationToken)
    => await gateUnitOfWork.Gates.GetCountAsync(specification, cancellationToken);
}
