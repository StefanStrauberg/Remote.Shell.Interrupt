namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetAll;

public record GetNetworkDevicesQuery() : IQuery<IEnumerable<NetworkDeviceDTO>>;
