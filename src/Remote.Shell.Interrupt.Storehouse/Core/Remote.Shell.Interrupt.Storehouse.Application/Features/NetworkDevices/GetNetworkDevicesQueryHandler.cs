namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices;

public record GetNetworkDevicesQuery() : IQuery<IEnumerable<NetworkDevice>>;

internal class GetNetworkDevicesQueryHandler(INetworkDeviceRepository networkDeviceRepository)
  : IQueryHandler<GetNetworkDevicesQuery, IEnumerable<NetworkDevice>>
{
  readonly INetworkDeviceRepository _networkDeviceRepository = networkDeviceRepository
    ?? throw new ArgumentNullException(nameof(networkDeviceRepository));

  async Task<IEnumerable<NetworkDevice>> IRequestHandler<GetNetworkDevicesQuery, IEnumerable<NetworkDevice>>.Handle(GetNetworkDevicesQuery request,
                                                                                                                    CancellationToken cancellationToken)
  {
    var networkDevices = await _networkDeviceRepository.GetAllAsync(cancellationToken);
    return networkDevices;
  }
}
