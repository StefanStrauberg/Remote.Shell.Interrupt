namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetNetworkDevicesByFilter;

/// <summary>
/// Represents a query to retrieve network devices based on filtering criteria.
/// </summary>
/// <param name="RequestParameters">The request parameters containing filtering and pagination settings.</param>
public record GetNetworkDevicesByFilterQuery(RequestParameters RequestParameters) : IQuery<PagedList<NetworkDeviceDTO>>;

/// <summary>
/// Handles the GetNetworkDevicesByFilterQuery and retrieves filtered network devices.
/// </summary>
/// <remarks>
/// This handler applies filtering expressions, builds the necessary specifications,
/// manages pagination, retrieves network devices, and returns the mapped results.
/// </remarks>
/// <param name="netDevUnitOfWork">Unit of work for network device-related database operations.</param>
/// <param name="specification">Specification used for filtering network device entities.</param>
/// <param name="queryFilterParser">Parser for processing filter expressions.</param>
/// <param name="mapper">Object mapper for DTO transformation.</param>
internal class GetNetworkDevicesByFilterQueryHandler(INetDevUnitOfWork netDevUnitOfWork,
                                                     INetworkDeviceSpecification specification,
                                                     IQueryFilterParser queryFilterParser,
                                                     IMapper mapper)
  : IQueryHandler<GetNetworkDevicesByFilterQuery, PagedList<NetworkDeviceDTO>>
{
  /// <summary>
  /// Handles the request to retrieve network devices based on filtering criteria.
  /// </summary>
  /// <param name="request">The query request containing filtering and pagination parameters.</param>
  /// <param name="cancellationToken">Token to support request cancellation.</param>
  /// <returns>A paginated list of network device DTOs.</returns>
  async Task<PagedList<NetworkDeviceDTO>> IRequestHandler<GetNetworkDevicesByFilterQuery, PagedList<NetworkDeviceDTO>>.Handle(GetNetworkDevicesByFilterQuery request,
                                                                                                                              CancellationToken cancellationToken)
  {
    // Parse the filter expression to extract filtering conditions
    var filterExpr = queryFilterParser.ParseFilters<NetworkDevice>(request.RequestParameters
                                                                   .Filters);

    // Build the base specification with filtering applied
    var baseSpec = BuildSpecification(specification,
                                      filterExpr);

    // Create a specification for counting total matching records
    var countSpec = baseSpec.Clone();

    // Pagination parameters
    var pageNumber = request.RequestParameters.PageNumber ?? 0;
    var pageSize = request.RequestParameters.PageSize ?? 0;

    // Apply pagination settings if enabled
    if (request.RequestParameters.EnablePagination)
        baseSpec.WithPagination(pageNumber,
                                pageSize);

    // Retrieve the list of network devices based on specifications
    var networkDevices = await netDevUnitOfWork.NetworkDevices
                                               .GetManyShortAsync(baseSpec,
                                                                  cancellationToken);

    // If no network devices are found, return an empty paginated list
    if (!networkDevices.Any())
      return new PagedList<NetworkDeviceDTO>([],0,0,0);

    // Retrieve the total count of matching records
    var count = await netDevUnitOfWork.NetworkDevices
                                      .GetCountAsync(countSpec,
                                                     cancellationToken);

    // Convert retrieved network device entities to DTOs
    var result = mapper.Map<List<NetworkDeviceDTO>>(networkDevices);

    // Return the paginated list of DTOs
    return new PagedList<NetworkDeviceDTO>(result,
                                           count,
                                           pageNumber,
                                           pageSize);
  }

  /// <summary>
  /// Builds the specification by applying filtering criteria.
  /// </summary>
  /// <param name="baseSpec">The base network device specification.</param>
  /// <param name="filterExpr">The filter expression to apply.</param>
  /// <returns>An updated specification with filtering applied.</returns>
  static INetworkDeviceSpecification BuildSpecification(INetworkDeviceSpecification baseSpec,
                                                        Expression<Func<NetworkDevice, bool>>? filterExpr)
  {
    var spec = baseSpec;

    if (filterExpr is not null)
        spec.AddFilter(filterExpr);

    return spec;
  }
}
