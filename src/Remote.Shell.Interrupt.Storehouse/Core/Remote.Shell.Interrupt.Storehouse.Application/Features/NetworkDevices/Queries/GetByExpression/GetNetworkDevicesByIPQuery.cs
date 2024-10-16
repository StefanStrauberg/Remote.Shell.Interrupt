namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetByExpression;

public record GetNetworkDevicesByIPQuery(string IpAddress)
  : IQuery<IEnumerable<NetworkDeviceDTO>>;
