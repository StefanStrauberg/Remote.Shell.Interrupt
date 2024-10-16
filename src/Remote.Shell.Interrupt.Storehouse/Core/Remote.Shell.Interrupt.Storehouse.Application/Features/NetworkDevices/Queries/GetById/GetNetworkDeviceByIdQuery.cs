namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetById;

public record GetNetworkDeviceByIdQuery(Guid Id) : IQuery<NetworkDeviceDTO>;
