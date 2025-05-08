namespace Remote.Shell.Interrupt.Storehouse.AppLogger;

/// <summary>
/// Implements an application logger using Microsoft.Extensions.Logging.
/// </summary>
/// <typeparam name="T">The type associated with the logger instance.</typeparam>
internal class AppLogger<T>(ILoggerFactory loggerFactory) 
  : IAppLogger<T>
{
  /// <summary>
  /// The logger instance used for logging messages.
  /// </summary>
  readonly ILogger<T> _logger = loggerFactory.CreateLogger<T>()
      ?? throw new ArgumentNullException(nameof(loggerFactory));  
  /// <summary>
  /// Logs an informational message.
  /// </summary>
  /// <param name="message">The message to log.</param>
  /// <param name="args">Optional arguments for message formatting.</param>
  void IAppLogger<T>.LogInformation(string message, params object[] args)
  {
    if (args.Length == 0)
        _logger.LogInformation("Message: {Message}", message);
    else
        _logger.LogInformation("Message: {Message}\nArgs: {Args}", message, args);
  }  
  /// <summary>
  /// Logs a warning message.
  /// </summary>
  /// <param name="message">The warning message to log.</param>
  /// <param name="args">Optional arguments for message formatting.</param>
  void IAppLogger<T>.LogWarning(string message, params object[] args)
  {
    if (args.Length == 0)
        _logger.LogWarning("Message: {Message}", message);
    else
        _logger.LogWarning("Message: {Message}\nArgs: {Args}", message, args);
  }  
  /// <summary>
  /// Logs an error message.
  /// </summary>
  /// <param name="message">The error message to log.</param>
  /// <param name="args">Optional arguments for message formatting.</param>
  void IAppLogger<T>.LogError(string message, params object[] args)
  {
    if (args.Length == 0)
      _logger.LogError("Message: {Message}", message);
    else
      _logger.LogError("Message: {Message}\nArgs: {Args}", message, args);
  }
}

/// <summary>
/// Provides a centralized logging mechanism using <see cref="ILoggerFactory"/>.
/// </summary>
internal class AppLogger(ILoggerFactory loggerFactory) 
  : IAppLogger
{
  /// <summary>
  /// Factory instance used to create loggers.
  /// </summary>
  private readonly ILoggerFactory _loggerFactory = loggerFactory
    ?? throw new ArgumentNullException(nameof(loggerFactory));   
  /// <summary>
  /// Logs an informational message.
  /// </summary>
  /// <param name="className">The name of the class where the log originates.</param>
  /// <param name="message">The log message format string.</param>
  /// <param name="args">The arguments to be formatted into the message.</param>
  public void LogInformation(string className, string message, params object[] args)
  {
    var logger = _loggerFactory.CreateLogger(className);
  
    if (args.Length == 0)
      logger.LogInformation("Message: {Message}", message);
    else
      logger.LogInformation("Message: {Message}\nArgs: {Args}", message, args);
  }    
  /// <summary>
  /// Logs a warning message.
  /// </summary>
  /// <param name="className">The name of the class where the log originates.</param>
  /// <param name="message">The log message format string.</param>
  /// <param name="args">The arguments to be formatted into the message.</param>
  public void LogWarning(string className, string message, params object[] args)
  {
    var logger = _loggerFactory.CreateLogger(className); 

    if (args.Length == 0)
      logger.LogWarning("Message: {Message}", message);
    else
      logger.LogWarning("Message: {Message}\nArgs: {Args}", message, args);
  }

  /// <summary>
  /// Logs an error message.
  /// </summary>
  /// <param name="className">The name of the class where the log originates.</param>
  /// <param name="message">The log message format string.</param>
  /// <param name="args">The arguments to be formatted into the message.</param>
  public void LogError(string className, string message, params object[] args)
  {
    var logger = _loggerFactory.CreateLogger(className);

    if (args.Length == 0)
      logger.LogError("Message: {Message}", message);
    else
      logger.LogError("Message: {Message}\nArgs: {Args}", message, args);
  }
}