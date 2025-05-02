namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

/// <summary>
/// Represents a bad request exception specific to SNMP operations.
/// </summary>
public class SNMPBadRequestException : BadRequestException
{
  /// <summary>
  /// Initializes a new instance of the <see cref="SNMPBadRequestException"/> class with a specified message.
  /// </summary>
  /// <param name="message">The message describing the exception.</param>
  public SNMPBadRequestException(string message)
    : base(message)
  { }

  /// <summary>
  /// Initializes a new instance of the <see cref="SNMPBadRequestException"/> class with a specified message and inner exception.
  /// </summary>
  /// <param name="message">The message describing the exception.</param>
  /// <param name="innerException">The inner exception providing additional context.</param>
  public SNMPBadRequestException(string message, Exception innerException)
    : base(message, innerException)
  { }
}
