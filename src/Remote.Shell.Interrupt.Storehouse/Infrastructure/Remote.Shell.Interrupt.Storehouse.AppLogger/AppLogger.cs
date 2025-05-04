namespace Remote.Shell.Interrupt.Storehouse.AppLogger;

/// <summary>
/// Implements an application logger using Microsoft.Extensions.Logging.
/// </summary>
/// <typeparam name="T">The type associated with the logger instance.</typeparam>
internal class AppLogger<T>(ILoggerFactory loggerFactory) : IAppLogger<T>
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
        => _logger.LogInformation("Message: {Message}. Args: {Args}", message, args);

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The warning message to log.</param>
    /// <param name="args">Optional arguments for message formatting.</param>
    void IAppLogger<T>.LogWarning(string message, params object[] args)
        => _logger.LogWarning("Message: {Message}. Args: {Args}", message, args);

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The error message to log.</param>
    /// <param name="args">Optional arguments for message formatting.</param>
    void IAppLogger<T>.LogError(string message, params object[] args)
        => _logger.LogError("Message: {Message}. Args: {Args}", message, args);
}

/// <summary>
/// Provides a centralized logging mechanism using <see cref="ILoggerFactory"/>.
/// </summary>
internal class AppLogger(ILoggerFactory loggerFactory) : IAppLogger
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
        // Проверка соответствия количества параметров
        if (args.Length == 0)
            logger.LogInformation("Class: {ClassName}, Message: {Message}", className, message);
        else
            logger.LogInformation("Class: {ClassName}, Message: {Message}", className, string.Format(message, args));
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
        logger.LogWarning("Class: {ClassName}, Message: {Message}", className, string.Format(message, args));
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
        logger.LogError("Class: {ClassName}, Message: {Message}", className, string.Format(message, args));
    }
}