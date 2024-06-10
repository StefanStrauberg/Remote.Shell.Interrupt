namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Exceptions;

public class SNMPBadRequestException(string Message) : BadRequestException(Message)
{ }
