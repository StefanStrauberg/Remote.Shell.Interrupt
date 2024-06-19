namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

public class NetworkDeviceExists(string Message) : BadRequestException(Message)
{ }

