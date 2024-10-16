namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.Create;

public record CreateNetworkDeviceCommand(string Host, string Community, string TypeOfNetworkDevice) : ICommand;