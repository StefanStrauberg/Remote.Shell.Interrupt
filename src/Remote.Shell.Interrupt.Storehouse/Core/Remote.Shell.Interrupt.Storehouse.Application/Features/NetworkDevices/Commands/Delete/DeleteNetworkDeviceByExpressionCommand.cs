namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.Delete;

public record DeleteNetworkDeviceByExpressionCommand(Expression<Func<NetworkDevice, bool>> FilterExpression)
  : ICommand;
