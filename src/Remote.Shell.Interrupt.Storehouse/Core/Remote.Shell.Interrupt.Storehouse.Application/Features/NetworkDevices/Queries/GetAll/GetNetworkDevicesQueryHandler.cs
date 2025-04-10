namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetAll;

public record GetNetworkDevicesQuery(RequestParameters RequestParameters) : IQuery<PagedList<NetworkDeviceDTO>>;

internal class GetNetworkDevicesQueryHandler(IUnitOfWork unitOfWork,
                                             IMapper mapper)
  : IQueryHandler<GetNetworkDevicesQuery, PagedList<NetworkDeviceDTO>>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<PagedList<NetworkDeviceDTO>> IRequestHandler<GetNetworkDevicesQuery, PagedList<NetworkDeviceDTO>>.Handle(GetNetworkDevicesQuery request,
                                                                                                                      CancellationToken cancellationToken)
  {
    var networkDevices = await _unitOfWork.NetworkDevices
                                          .GetNetworkDevicesByQueryAsync(request.RequestParameters,
                                                                         cancellationToken);

    var count = await _unitOfWork.NetworkDevices
                                 .GetCountAsync(request.RequestParameters,
                                                cancellationToken);

    if (!networkDevices.Any())
      return new PagedList<NetworkDeviceDTO>([],0,0,0);

    var result = _mapper.Map<List<NetworkDeviceDTO>>(networkDevices);

    return new PagedList<NetworkDeviceDTO>(result,
                                           count,
                                           request.RequestParameters.PageNumber,
                                           request.RequestParameters.PageSize);
  }
}
