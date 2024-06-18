namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices;

public record GetNetworkDeviceByExpressionQuery(Expression<Func<NetworkDevice, bool>> FilterExpression) : IQuery<NetworkDevice>;

internal class GetNetworkDeviceByExpressionQueryHandler(INetworkDeviceRepository networkDeviceRepository)
  : IQueryHandler<GetNetworkDeviceByExpressionQuery, NetworkDevice>
{
  readonly INetworkDeviceRepository _networkDeviceRepository = networkDeviceRepository
    ?? throw new ArgumentNullException(nameof(networkDeviceRepository));

  async Task<NetworkDevice> IRequestHandler<GetNetworkDeviceByExpressionQuery, NetworkDevice>.Handle(GetNetworkDeviceByExpressionQuery request,
                                                                                                     CancellationToken cancellationToken)
  {
    var networkDevice = await _networkDeviceRepository.FindOneAsync(request.FilterExpression, cancellationToken)
      ?? throw new NetworkDeviceNotFoundException(request.FilterExpression.Name!);
    return networkDevice;
  }
}
