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
    // // Parse the filter expression to extract filtering conditions
    // var filterExpr = queryFilterParser.ParseFilters<NetworkDevice>(request.RequestParameters
    //                                                                .Filters);

    // // Build the base specification with filtering applied
    // var baseSpec = BuildSpecification(specification,
    //                                   filterExpr);

    // // Create a specification for counting total matching records
    // var countSpec = baseSpec.Clone();

    // // Pagination parameters
    // var pageNumber = request.RequestParameters.PageNumber ?? 0;
    // var pageSize = request.RequestParameters.PageSize ?? 0;

    // // Apply pagination settings if enabled
    // if (request.RequestParameters.IsPaginated)
    //   baseSpec.ConfigurePagination(pageNumber, pageSize);

    // // Retrieve the list of network devices based on specifications
    // var networkDevices = await netDevUnitOfWork.NetworkDevices
    //                                            .GetManyShortAsync(baseSpec,
    //                                                               cancellationToken);

    // // If no network devices are found, return an empty paginated list
    // if (!networkDevices.Any())
    //   return new PagedList<NetworkDeviceDTO>([], 0, 0, 0);

    // // Retrieve the total count of matching records
    // var count = await netDevUnitOfWork.NetworkDevices
    //                                   .GetCountAsync(countSpec,
    //                                                  cancellationToken);

    // // Convert retrieved network device entities to DTOs
    // var result = mapper.Map<List<NetworkDeviceDTO>>(networkDevices);

    // // Return the paginated list of DTOs
    // return new PagedList<NetworkDeviceDTO>(result,
    //                                        count,
    //                                        pageNumber,
    //                                        pageSize);
    return await Task.FromResult<PagedList<NetworkDeviceDTO>>(new PagedList<NetworkDeviceDTO>([],0,new(0,0)));
  }

  // static INetworkDeviceSpecification BuildSpecification(INetworkDeviceSpecification baseSpec,
  //                                                       Expression<Func<NetworkDevice, bool>>? filterExpr)
  // {
  //   var spec = baseSpec;

  //   if (filterExpr is not null)
  //       spec.AddFilter(filterExpr);

  //   return spec;
  // }
}
