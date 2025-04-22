namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetAll;

public record GetNetworkDevicesQuery(RequestParameters RequestParameters) : IQuery<PagedList<NetworkDeviceDTO>>;

internal class GetNetworkDevicesQueryHandler(INetDevUnitOfWork netDevUnitOfWork,
                                             IMapper mapper)
  : IQueryHandler<GetNetworkDevicesQuery, PagedList<NetworkDeviceDTO>>
{
  async Task<PagedList<NetworkDeviceDTO>> IRequestHandler<GetNetworkDevicesQuery, PagedList<NetworkDeviceDTO>>.Handle(GetNetworkDevicesQuery request,
                                                                                                                      CancellationToken cancellationToken)
  {
    var networkDevices = await netDevUnitOfWork.NetworkDevices
                                               .GetManyShortAsync(request.RequestParameters,
                                                                              cancellationToken);

    var count = await netDevUnitOfWork.NetworkDevices
                                      .GetCountAsync(request.RequestParameters,
                                                     cancellationToken);

    if (!networkDevices.Any())
      return default!;

    var result = mapper.Map<List<NetworkDeviceDTO>>(networkDevices);

    return new PagedList<NetworkDeviceDTO>(result,
                                           count,
                                           request.RequestParameters.PageNumber,
                                           request.RequestParameters.PageSize);
  }
}
