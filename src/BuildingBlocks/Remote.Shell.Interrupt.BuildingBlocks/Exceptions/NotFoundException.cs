namespace Remote.Shell.Interrupt.BuildingBlocks.Exceptions;

public abstract class NotFoundException(string message) : ApplicationException("Not Found", message)
{ }
