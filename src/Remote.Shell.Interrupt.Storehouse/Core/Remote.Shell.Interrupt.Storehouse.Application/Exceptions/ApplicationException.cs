namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

/// <summary>
/// Represents a base application exception with a title and message.
/// </summary>
public abstract class ApplicationException : Exception
{
  /// <summary>
  /// Gets the title of the exception.
  /// </summary>
  public string Title { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="ApplicationException"/> class with a title and message.
  /// </summary>
  /// <param name="title">The title of the exception.</param>
  /// <param name="message">The message describing the exception.</param>
  protected ApplicationException(string title, string message)
        : base(message)
  {
    Title = title;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ApplicationException"/> class with a title, message, and inner exception.
  /// </summary>
  /// <param name="title">The title of the exception.</param>
  /// <param name="message">The message describing the exception.</param>
  /// <param name="innerException">The inner exception providing more context.</param>
  protected ApplicationException(string title, string message, Exception innerException)
      : base(message, innerException)
  {
    Title = title;
  }
}
