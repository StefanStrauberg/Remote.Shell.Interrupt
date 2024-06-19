namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

public class GatewayNotFoundException(string Message) : BadRequestException(Message)
{ }