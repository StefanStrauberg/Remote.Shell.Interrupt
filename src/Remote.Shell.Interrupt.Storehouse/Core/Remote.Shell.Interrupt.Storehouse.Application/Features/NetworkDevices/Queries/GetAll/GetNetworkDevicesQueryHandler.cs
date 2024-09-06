namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetAll;

internal class GetNetworkDevicesQueryHandler(INetworkDeviceRepository networkDeviceRepository,
                                             IMapper mapper)
  : IQueryHandler<GetNetworkDevicesQuery, IEnumerable<NetworkDeviceDTO>>
{
  readonly INetworkDeviceRepository _networkDeviceRepository = networkDeviceRepository
    ?? throw new ArgumentNullException(nameof(networkDeviceRepository));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<IEnumerable<NetworkDeviceDTO>> IRequestHandler<GetNetworkDevicesQuery, IEnumerable<NetworkDeviceDTO>>.Handle(GetNetworkDevicesQuery request,
                                                                                                                          CancellationToken cancellationToken)
  {
    var networkDevices = await _networkDeviceRepository.GetAllAsync(cancellationToken);
    var networkDevicesDTOs = _mapper.Map<IEnumerable<NetworkDeviceDTO>>(networkDevices);

    return networkDevicesDTOs;
  }
}
