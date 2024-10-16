namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

public class SNMPBadRequestException : BadRequestException
{
  public SNMPBadRequestException(string message)
    : base(message)
  { }
  public SNMPBadRequestException(string message, Exception innerException)
    : base(message, innerException)
  { }
}
