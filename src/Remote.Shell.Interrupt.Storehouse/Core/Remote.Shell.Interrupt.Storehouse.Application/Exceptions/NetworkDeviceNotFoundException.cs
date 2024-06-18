namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

public class NetworkDeviceNotFoundException(string Message) : BadRequestException(Message)
{ }
