namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetAll;

public record GetNetworkDevicesQuery() : IQuery<IEnumerable<NetworkDeviceDTO>>;

internal class GetNetworkDevicesQueryHandler(IUnitOfWork unitOfWork,
                                             IMapper mapper)
  : IQueryHandler<GetNetworkDevicesQuery, IEnumerable<NetworkDeviceDTO>>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<IEnumerable<NetworkDeviceDTO>> IRequestHandler<GetNetworkDevicesQuery, IEnumerable<NetworkDeviceDTO>>.Handle(GetNetworkDevicesQuery request,
                                                                                                                          CancellationToken cancellationToken)
  {
    var networkDevices = await _unitOfWork.NetworkDevices
                                          .GetAllAsync(cancellationToken);

    var networkDevicesDTOs = _mapper.Map<IEnumerable<NetworkDeviceDTO>>(networkDevices);

    return networkDevicesDTOs;
  }
}
