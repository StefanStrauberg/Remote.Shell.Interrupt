namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientsWithChildrenByFilter;

/// <summary>
/// Represents a query to retrieve a filtered and paginated list of clients with their related entities included.
/// </summary>
/// <param name="Parameters">Encapsulates filtering conditions and pagination rules.</param>
public record GetClientsWithChildrenByFilterQuery(RequestParameters Parameters)
  : FindEntitiesByFilterQuery<DetailClientDTO>(Parameters);

/// <summary>
/// Handles <see cref="GetClientsWithChildrenByFilterQuery"/> by applying filters and pagination,
/// fetching client entities along with their related data, and mapping them to DTOs.
/// </summary>
/// <param name="locBillUnitOfWork">Provides repository access for client-related operations.</param>
/// <param name="specification">Clonable specification used to build filtering logic and includes.</param>
/// <param name="queryFilterParser">Converts filter descriptors into queryable expressions.</param>
/// <param name="mapper">Maps <see cref="Client"/> entities to <see cref="DetailClientDTO"/> objects.</param>
internal class GetClientsWithChildrenByFilterQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                                          IClientSpecification specification,
                                                          IQueryFilterParser queryFilterParser,
                                                          IMapper mapper)
  : FindEntitiesByFilterQueryHandler<Client, DetailClientDTO, GetClientsWithChildrenByFilterQuery>(specification, queryFilterParser, mapper)
{
  readonly IQueryFilterParser _queryFilterParser = queryFilterParser;

  /// <summary>
  /// Calculates the number of client entities that satisfy the provided filtering specification.
  /// </summary>
  /// <param name="specification">The specification containing filtering logic.</param>
  /// <param name="cancellationToken">Token used to propagate cancellation signals.</param>
  /// <returns>The total count of matching client entities.</returns>
  protected override async Task<int> CountResultsAsync(ISpecification<Client> specification,
                                                       CancellationToken cancellationToken)
    => await locBillUnitOfWork.Clients.GetCountAsync(specification, cancellationToken);

  /// <summary>
  /// Retrieves a collection of client entities along with their related children, filtered by the provided specification.
  /// </summary>
  /// <param name="specification">Specification used to query and include related data.</param>
  /// <param name="cancellationToken">Token used to observe cancellation requests.</param>
  /// <returns>A collection of <see cref="Client"/> entities with related children included.</returns>
  protected override async Task<IEnumerable<Client>> FetchEntitiesAsync(ISpecification<Client> specification,
                                                                        CancellationToken cancellationToken)
    => await locBillUnitOfWork.Clients.GetManyWithChildrenAsync(specification, cancellationToken);

  /// <summary>
  /// Constructs a filtering specification that includes related client entities such as COD, TfPlan, and SPRVlans.
  /// </summary>
  /// <param name="requestParameters">The parameters defining filter criteria.</param>
  /// <returns>A specification object configured with includes and filters.</returns>
  protected override ISpecification<Client> BuildSpecification(RequestParameters requestParameters)
  {
    var filterExpr = _queryFilterParser.ParseFilters<Client>(requestParameters.Filters);

    var spec = specification.AddInclude(c => c.COD)
                            .AddInclude(c => c.TfPlan!)
                            .AddInclude(c => c.SPRVlans);

    if (filterExpr is not null)
      spec.AddFilter(filterExpr);

    return spec;
  }
}
