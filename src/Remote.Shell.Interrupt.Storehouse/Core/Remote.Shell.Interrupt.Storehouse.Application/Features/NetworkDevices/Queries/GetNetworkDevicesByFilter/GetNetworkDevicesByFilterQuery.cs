namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetNetworkDevicesByFilter;

/// <summary>
/// Represents a query to retrieve a filtered and paginated collection of network devices.
/// </summary>
/// <param name="Parameters">Contains filtering and pagination parameters.</param>
public record GetNetworkDevicesByFilterQuery(RequestParameters Parameters)
  : FindEntitiesByFilterQuery<NetworkDeviceDTO>(Parameters);

/// <summary>
/// Handles <see cref="GetNetworkDevicesByFilterQuery"/> by applying filtering and pagination logic,
/// querying network device entities, and mapping them to DTOs.
/// </summary>
/// <param name="netDevUnitOfWork">Provides access to network device repositories.</param>
/// <param name="specification">Clonable specification used to layer filter logic.</param>
/// <param name="queryFilterParser">Parses textual filters into executable expressions.</param>
/// <param name="mapper">Maps domain <see cref="NetworkDevice"/> entities to <see cref="NetworkDeviceDTO"/> representations.</param>
internal class GetNetworkDevicesByFilterQueryHandler(INetDevUnitOfWork netDevUnitOfWork,
                                                     INetworkDeviceSpecification specification,
                                                     IQueryFilterParser queryFilterParser,
                                                     IMapper mapper)
  : FindEntitiesByFilterQueryHandler<NetworkDevice, NetworkDeviceDTO, GetNetworkDevicesByFilterQuery>(specification, queryFilterParser, mapper)
{
  /// <summary>
  /// Calculates the number of <see cref="NetworkDevice"/> entities that match the given specification.
  /// </summary>
  /// <param name="specification">The filtering specification.</param>
  /// <param name="cancellationToken">Token used to propagate cancellation signals.</param>
  /// <returns>The total count of matching network device records.</returns>
  protected override async Task<int> CountResultsAsync(ISpecification<NetworkDevice> specification,
                                                       CancellationToken cancellationToken)
    => await netDevUnitOfWork.NetworkDevices.GetCountAsync(specification, cancellationToken);

  /// <summary>
  /// Retrieves a filtered collection of <see cref="NetworkDevice"/> entities using the provided specification.
  /// </summary>
  /// <param name="specification">The specification used to filter the results.</param>
  /// <param name="cancellationToken">Token used to observe cancellation signals.</param>
  /// <returns>A collection of network devices matching the filtering criteria.</returns>
  protected override async Task<IEnumerable<NetworkDevice>> FetchEntitiesAsync(ISpecification<NetworkDevice> specification,
                                                                               CancellationToken cancellationToken)
    => await netDevUnitOfWork.NetworkDevices.GetManyShortAsync(specification, cancellationToken);
}
