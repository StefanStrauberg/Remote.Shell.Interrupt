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