namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Logger;

/// <summary>
/// Generic logger interface for structured application logging.
/// </summary>
/// <typeparam name="T">The type associated with the logger instance.</typeparam>
public interface IAppLogger<T>
{
  /// <summary>
  /// Logs an informational message.
  /// </summary>
  /// <param name="message">The message to log.</param>
  /// <param name="args">Optional arguments for message formatting.</param>
  void LogInformation(string message, params object[] args);

  /// <summary>
  /// Logs a warning message.
  /// </summary>
  /// <param name="message">The warning message to log.</param>
  /// <param name="args">Optional arguments for message formatting.</param>
  void LogWarning(string message, params object[] args);

  /// <summary>
  /// Logs an error message.
  /// </summary>
  /// <param name="message">The error message to log.</param>
  /// <param name="args">Optional arguments for message formatting.</param>
  void LogError(string message, params object[] args);
}

/// <summary>
/// Logger interface for structured application logging.
/// </summary>
public interface IAppLogger
{
  /// <summary>
  /// Logs an informational message with optional formatting arguments.
  /// </summary>
  /// <param name="className">The name of the class invoking the log entry.</param>
  /// <param name="message">The log message, optionally parameterized.</param>
  /// <param name="args">Optional arguments to be included in the message template.</param>
  void LogInformation(string className, string message, params object[] args);

  /// <summary>
  /// Logs a warning message with optional formatting arguments.
  /// </summary>
  /// <param name="className">The name of the class invoking the log entry.</param>
  /// <param name="message">The warning message, optionally parameterized.</param>
  /// <param name="args">Optional arguments to be included in the message template.</param>
  void LogWarning(string className, string message, params object[] args);

  /// <summary>
  /// Logs an error message with optional formatting arguments.
  /// </summary>
  /// <param name="className">The name of the class invoking the log entry.</param>
  /// <param name="message">The error message, optionally parameterized.</param>
  /// <param name="args">Optional arguments to be included in the message template.</param>
  void LogError(string className, string message, params object[] args);
}