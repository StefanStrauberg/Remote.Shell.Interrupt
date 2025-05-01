namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetNetworkDevicesByFilter;

public record GetNetworkDevicesByFilterQuery(RequestParameters RequestParameters) : IQuery<PagedList<NetworkDeviceDTO>>;

internal class GetNetworkDevicesByFilterQueryHandler(INetDevUnitOfWork netDevUnitOfWork,
                                                     INetworkDeviceSpecification specification,
                                                     IQueryFilterParser queryFilterParser,
                                                     IMapper mapper)
  : IQueryHandler<GetNetworkDevicesByFilterQuery, PagedList<NetworkDeviceDTO>>
{
  async Task<PagedList<NetworkDeviceDTO>> IRequestHandler<GetNetworkDevicesByFilterQuery, PagedList<NetworkDeviceDTO>>.Handle(GetNetworkDevicesByFilterQuery request,
                                                                                                                              CancellationToken cancellationToken)
  {
    // Parse filter
    var filterExpr = queryFilterParser.ParseFilters<NetworkDevice>(request.RequestParameters
                                                                   .Filters);

    // Build base specification
    var baseSpec = BuildSpecification(specification,
                                      filterExpr);

    // Count records (without pagination)
    var countSpec = (INetworkDeviceSpecification)baseSpec.Clone();
    var count = await netDevUnitOfWork.NetworkDevices
                                      .GetCountAsync(countSpec,
                                                     cancellationToken);

    // Pagination parameters
    var pageNumber = request.RequestParameters.PageNumber ?? 0;
    var pageSize = request.RequestParameters.PageSize ?? 0;

    if (request.RequestParameters.EnablePagination)
        baseSpec.WithPagination(pageNumber,
                                pageSize);

    var networkDevices = await netDevUnitOfWork.NetworkDevices
                                               .GetManyShortAsync(baseSpec,
                                                                  cancellationToken);

    if (!networkDevices.Any())
      return new PagedList<NetworkDeviceDTO>([],0,0,0);

    var result = mapper.Map<List<NetworkDeviceDTO>>(networkDevices);

    // Return results
    return new PagedList<NetworkDeviceDTO>(result,
                                           count,
                                           pageNumber,
                                           pageSize);
  }

  static INetworkDeviceSpecification BuildSpecification(INetworkDeviceSpecification baseSpec,
                                                        Expression<Func<NetworkDevice, bool>>? filterExpr)
  {
    var spec = baseSpec;

    if (filterExpr is not null)
        spec.AddFilter(filterExpr);

    return spec;
  }
}
