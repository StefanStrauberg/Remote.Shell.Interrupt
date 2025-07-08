namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientById;

/// <summary>
/// Represents a query for retrieving detailed information about a single client entity
/// by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the client to retrieve.</param>
public record GetClientByIdQuery(Guid Id)
  : FindEntityByIdQuery<DetailClientDTO>(Id);

/// <summary>
/// Handles <see cref="GetClientByIdQuery"/> by constructing a specification with related data,
/// verifying existence, and retrieving the client entity from the database.
/// </summary>
/// <param name="locBillUnitOfWork">Provides access to client-related data sources.</param>
/// <param name="specification">Clonable base specification for client filtering and data inclusion.</param>
/// <param name="queryFilterParser">Parses filter descriptors into expression trees.</param>
/// <param name="mapper">Transforms domain entities into data transfer objects.</param>
internal class GetClientByIdQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                         IClientSpecification specification,
                                         IQueryFilterParser queryFilterParser,
                                         IMapper mapper)
  : FindEntityByIdQueryHandler<Client, DetailClientDTO, GetClientByIdQuery>(specification, queryFilterParser, mapper)
{
  private readonly IQueryFilterParser _queryFilterParser = queryFilterParser;

  /// <summary>
  /// Constructs a specification to retrieve the client entity and its related data.
  /// </summary>
  /// <param name="clientId">The unique identifier of the client to fetch.</param>
  /// <returns>A specification that includes related entities and filters by ID.</returns>
  protected override ISpecification<Client> BuildSpecification(Guid clientId)
  {
    var filterExpr = _queryFilterParser.ParseFilters<Client>(RequestParameters.ForId(clientId).Filters);

    var spec = specification.AddInclude(c => c.COD)
                            .AddInclude(c => c.TfPlan!)
                            .AddInclude(c => c.SPRVlans)
                            .Clone();

    if (filterExpr is not null)
      spec.AddFilter(filterExpr);

    return spec;
  }

  /// <summary>
  /// Ensures that a client entity matching the specification exists; otherwise, throws an exception.
  /// </summary>
  /// <param name="specification">The specification used to validate entity existence.</param>
  /// <param name="cancellationToken">Token used to observe cancellation signals.</param>
  /// <exception cref="EntityNotFoundException">Thrown if the client entity cannot be found.</exception>
  protected override async Task EnsureEntityExistAsync(ISpecification<Client> specification,
                                                       CancellationToken cancellationToken)
  {
    bool exists = await locBillUnitOfWork.Clients.AnyByQueryAsync(specification, cancellationToken);

    if (exists is not true)
      throw new EntityNotFoundException(typeof(Client), specification.ToString() ?? string.Empty);
  }

  /// <summary>
  /// Retrieves the client entity and its associated children using the provided specification.
  /// </summary>
  /// <param name="specification">Specification used to query the client.</param>
  /// <param name="cancellationToken">Used to propagate cancellation requests.</param>
  /// <returns>The retrieved <see cref="Client"/> entity with related data.</returns>
  protected override async Task<Client> FetchEntityAsync(ISpecification<Client> specification,
                                                         CancellationToken cancellationToken)
    => await locBillUnitOfWork.Clients.GetOneWithChildrenAsync(specification, cancellationToken);
}