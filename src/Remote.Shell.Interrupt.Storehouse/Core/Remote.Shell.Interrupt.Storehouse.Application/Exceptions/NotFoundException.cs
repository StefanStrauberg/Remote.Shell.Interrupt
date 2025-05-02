namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

/// <summary>
/// Represents an exception indicating that a requested entity was not found.
/// </summary>
/// <param name="message">The message describing the exception.</param>
public abstract class NotFoundException(string message) : ApplicationException("Not Found", message)
{ }
