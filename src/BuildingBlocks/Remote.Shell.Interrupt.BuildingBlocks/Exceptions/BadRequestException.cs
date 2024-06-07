namespace Remote.Shell.Interrupt.BuildingBlocks.Exceptions;

public abstract class BadRequestException(string message) : ApplicationException("Bad Request", message)
{ }
