namespace Remote.Shell.Interrupt.Storehouse.Application.Features.SPRVlans.Queries.GetSPRVlansByFilter;

/// <summary>
/// Defines a query for retrieving a paginated and optionally filtered list of <see cref="SPRVlanDTO"/> records.
/// </summary>
/// <param name="Parameters">Encapsulates filter expressions and pagination metadata.</param>
public sealed record GetSPRVlansByFilterQuery(RequestParameters Parameters)
  : FindEntitiesByFilterQuery<SPRVlanDTO>(Parameters);

/// <summary>
/// Handles <see cref="GetSPRVlansByFilterQuery"/> by querying, counting, and mapping <see cref="SPRVlan"/> entities.
/// </summary>
/// <param name="locBillUnitOfWork">Unit of work providing access to SPRVlan-related repositories.</param>
/// <param name="specification">Clonable specification object used to layer filter logic.</param>
/// <param name="queryFilterParser">Converts textual filters into queryable expressions.</param>
/// <param name="mapper">Maps domain entities to <see cref="SPRVlanDTO"/> instances.</param>
internal class GetSPRVlansByFilterQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                               ISPRVlanSpecification specification,
                                               IQueryFilterParser queryFilterParser,
                                               IMapper mapper)
  : FindEntitiesByFilterQueryHandler<SPRVlan, SPRVlanDTO, GetSPRVlansByFilterQuery>(specification, queryFilterParser, mapper)
{
  /// <summary>
  /// Retrieves all <see cref="SPRVlan"/> entities that match the filtering specification.
  /// </summary>
  /// <param name="spec">Filter specification for SPRVlan querying.</param>
  /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
  /// <returns>A collection of matching <see cref="SPRVlan"/> entities.</returns>
  protected override async Task<IEnumerable<SPRVlan>> FetchEntitiesAsync(ISpecification<SPRVlan> spec,
                                                                         CancellationToken cancellationToken)
    => await locBillUnitOfWork.SPRVlans.GetManyShortAsync(spec, cancellationToken);

  /// <summary>
  /// Calculates the total number of <see cref="SPRVlan"/> entities that satisfy the provided filter.
  /// </summary>
  /// <param name="spec">Specification defining filtering rules.</param>
  /// <param name="cancellationToken">Used to observe cancellation signals.</param>
  /// <returns>The total count of matching <see cref="SPRVlan"/> entities.</returns>  
  protected override async Task<int> CountResultsAsync(ISpecification<SPRVlan> spec,
                                                       CancellationToken cancellationToken)
    => await locBillUnitOfWork.SPRVlans.GetCountAsync(spec, cancellationToken);
}
