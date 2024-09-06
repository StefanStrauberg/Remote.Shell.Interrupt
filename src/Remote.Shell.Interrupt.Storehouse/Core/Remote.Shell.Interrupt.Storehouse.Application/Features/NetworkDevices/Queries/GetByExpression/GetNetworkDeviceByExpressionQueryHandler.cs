namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetByExpression;

internal class GetNetworkDeviceByExpressionQueryHandler(INetworkDeviceRepository networkDeviceRepository)
  : IQueryHandler<GetNetworkDeviceByExpressionQuery, NetworkDevice>
{
  readonly INetworkDeviceRepository _networkDeviceRepository = networkDeviceRepository
    ?? throw new ArgumentNullException(nameof(networkDeviceRepository));

  async Task<NetworkDevice> IRequestHandler<GetNetworkDeviceByExpressionQuery, NetworkDevice>.Handle(GetNetworkDeviceByExpressionQuery request,
                                                                                                     CancellationToken cancellationToken)
  {
    var networkDevice = await _networkDeviceRepository.FindOneAsync(filterExpression: request.FilterExpression,
                                                                    cancellationToken: cancellationToken)
      ?? throw new EntityNotFoundException(request.FilterExpression.Name!);

    return networkDevice;
  }
}
