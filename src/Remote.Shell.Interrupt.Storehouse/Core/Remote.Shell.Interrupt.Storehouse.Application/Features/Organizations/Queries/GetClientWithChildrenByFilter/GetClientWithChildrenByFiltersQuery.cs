namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientWithChildrenByFilter;

/// <summary>
/// Represents a query to retrieve a single client entity along with its related children,
/// based on filter criteria supplied in <see cref="RequestParameters"/>.
/// </summary>
/// <param name="Parameters">The filter descriptors used to locate and shape the query result.</param>
public record GetClientWithChildrenByFilterQuery(RequestParameters Parameters)
  : FindEntityByFilterQuery<DetailClientDTO>(Parameters);

/// <summary>
/// Handles <see cref="GetClientWithChildrenByFilterQuery"/> by composing filtering logic,
/// validating entity existence, fetching relational data, and mapping it to a DTO.
/// </summary>
/// <param name="locBillUnitOfWork">Unit of work providing access to client-related repositories.</param>
/// <param name="specification">Base specification used to enrich queries with includes and filters.</param>
/// <param name="queryFilterParser">Parses request filters into executable query expressions.</param>
/// <param name="mapper">Maps domain <see cref="Client"/> entities to <see cref="DetailClientDTO"/> results.</param>
internal class GetClientWithChildrenByFilterHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                                    IClientSpecification specification,
                                                    IQueryFilterParser queryFilterParser,
                                                    IMapper mapper)
  : FindEntityByFilterQueryHandler<Client, DetailClientDTO, GetClientWithChildrenByFilterQuery>(specification, queryFilterParser, mapper)
{
  readonly IQueryFilterParser _queryFilterParser = queryFilterParser;

  /// <summary>
  /// Builds a specification for querying the client entity, including related data and filtering rules.
  /// </summary>
  /// <param name="requestParameters">The parameters defining filters and query intent.</param>
  /// <returns>A specification configured with includes and filters.</returns>
  protected override ISpecification<Client> BuildSpecification(RequestParameters requestParameters)
  {
    var filterExpr = _queryFilterParser.ParseFilters<Client>(requestParameters.Filters);

    var spec = specification.AddInclude(c => c.COD)
                            .AddInclude(c => c.TfPlan)
                            .AddInclude(c => c.SPRVlans)
                            .Clone();

    if (filterExpr is not null)
      spec.AddFilter(filterExpr);

    return spec;
  }

  /// <summary>
  /// Ensures that a client entity exists matching the specification; otherwise, throws an exception.
  /// </summary>
  /// <param name="specification">Specification used to check for existence.</param>
  /// <param name="cancellationToken">Token for monitoring cancellation requests.</param>
  /// <exception cref="EntityNotFoundException">Thrown when no matching client entity is found.</exception>
  protected override async Task EnsureEntityExistAsync(ISpecification<Client> specification,
                                                       CancellationToken cancellationToken)
  {
    var existing = await locBillUnitOfWork.Clients.AnyByQueryAsync(specification, cancellationToken);

    if (existing is not true)
      throw new EntityNotFoundException(typeof(Client), specification.ToString() ?? string.Empty);
  }

  /// <summary>
  /// Retrieves the client entity along with its related children using the specified filtering logic.
  /// </summary>
  /// <param name="specification">Specification used to query and shape the client result.</param>
  /// <param name="cancellationToken">Token for task cancellation monitoring.</param>
  /// <returns>The fully populated <see cref="Client"/> entity.</returns>
  protected override async Task<Client> FetchEntityAsync(ISpecification<Client> specification,
                                                         CancellationToken cancellationToken)
    => await locBillUnitOfWork.Clients.GetOneWithChildrenAsync(specification, cancellationToken);
}