namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientsByFilter;

/// <summary>
/// Defines a query to retrieve a paginated and filtered collection of client summaries.
/// </summary>
/// <param name="Parameters">Contains filter descriptors and pagination settings.</param>
public record GetClientsByFilterQuery(RequestParameters Parameters)
  : FindEntitiesByFilterQuery<ShortClientDTO>(Parameters);

/// <summary>
/// Handles <see cref="GetClientsByFilterQuery"/> by applying filtering and pagination,
/// querying client entities, and mapping them into summary DTOs.
/// </summary>
/// <param name="locBillUnitOfWork">Provides access to client-related data operations.</param>
/// <param name="specification">Clonable specification used to layer filtering conditions.</param>
/// <param name="queryFilterParser">Parses textual filters into queryable expressions.</param>
/// <param name="mapper">Maps domain <see cref="Client"/> entities to <see cref="ShortClientDTO"/> representations.</param>
internal class GetClientsByFilterQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                              IClientSpecification specification,
                                              IQueryFilterParser queryFilterParser,
                                              IMapper mapper)
  : FindEntitiesByFilterQueryHandler<Client, ShortClientDTO, GetClientsByFilterQuery>(specification, queryFilterParser, mapper)
{
  /// <summary>
  /// Counts the number of <see cref="Client"/> entities matching the given specification.
  /// </summary>
  /// <param name="specification">Specification containing filtering logic.</param>
  /// <param name="cancellationToken">Token used to monitor for cancellation signals.</param>
  /// <returns>The total count of matching client records.</returns>
  protected override async Task<int> CountResultsAsync(ISpecification<Client> specification,
                                                       CancellationToken cancellationToken)
    => await locBillUnitOfWork.Clients.GetCountAsync(specification, cancellationToken);

  /// <summary>
  /// Retrieves a collection of <see cref="Client"/> entities using the provided filter specification.
  /// </summary>
  /// <param name="specification">Filtering specification used for querying entities.</param>
  /// <param name="cancellationToken">Token used to propagate cancellation.</param>
  /// <returns>A collection of client entities that match the filtering criteria.</returns>
  protected override async Task<IEnumerable<Client>> FetchEntitiesAsync(ISpecification<Client> specification,
                                                                        CancellationToken cancellationToken)
    => await locBillUnitOfWork.Clients.GetManyShortAsync(specification, cancellationToken);
}
