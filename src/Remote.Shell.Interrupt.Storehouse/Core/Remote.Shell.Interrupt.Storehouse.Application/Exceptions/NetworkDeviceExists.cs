namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

public class NetworkDeviceAlreadyExists(string Message) : BadRequestException(Message)
{ }

