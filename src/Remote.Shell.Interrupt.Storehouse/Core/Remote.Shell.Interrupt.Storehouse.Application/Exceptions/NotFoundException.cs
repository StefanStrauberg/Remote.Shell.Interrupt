namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

public abstract class NotFoundException(string message) : ApplicationException("Not Found", message)
{ }
