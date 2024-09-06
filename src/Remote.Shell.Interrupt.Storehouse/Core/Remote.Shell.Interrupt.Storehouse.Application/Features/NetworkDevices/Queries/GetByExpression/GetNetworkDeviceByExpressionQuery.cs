namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetByExpression;

public record GetNetworkDeviceByExpressionQuery(Expression<Func<NetworkDevice, bool>> FilterExpression) : IQuery<NetworkDevice>;
