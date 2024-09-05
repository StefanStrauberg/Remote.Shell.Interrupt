namespace Remote.Shell.Interrupt.BuildingBlocks.Exceptions;

public abstract class BadRequestException : ApplicationException
{
  protected BadRequestException(string message)
    : base("Bad Request", message)
  { }

  protected BadRequestException(string message, Exception innerException)
    : base("Bad Request", message, innerException)
  { }
}
