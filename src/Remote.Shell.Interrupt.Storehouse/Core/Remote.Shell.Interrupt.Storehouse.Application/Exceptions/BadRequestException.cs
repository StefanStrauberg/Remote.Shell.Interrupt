namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

/// <summary>
/// Represents an application exception for bad requests.
/// </summary>
public abstract class BadRequestException : ApplicationException
{
  /// <summary>
  /// Initializes a new instance of the <see cref="BadRequestException"/> class with a specified message.
  /// </summary>
  /// <param name="message">The message describing the exception.</param>
  protected BadRequestException(string message)
    : base("Bad Request", message)
  { }

  /// <summary>
  /// Initializes a new instance of the <see cref="BadRequestException"/> class with a specified message and inner exception.
  /// </summary>
  /// <param name="message">The message describing the exception.</param>
  /// <param name="innerException">The inner exception providing additional context.</param>
  protected BadRequestException(string message, Exception innerException)
    : base("Bad Request", message, innerException)
  { }
}
