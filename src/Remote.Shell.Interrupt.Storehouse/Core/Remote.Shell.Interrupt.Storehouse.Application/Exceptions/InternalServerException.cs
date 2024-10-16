namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

public class InternalServerException(string message) : ApplicationException("Internal Server Exception", message)
{ }
