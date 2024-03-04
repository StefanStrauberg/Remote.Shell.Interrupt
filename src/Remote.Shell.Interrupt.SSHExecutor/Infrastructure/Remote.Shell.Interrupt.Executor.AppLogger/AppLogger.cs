namespace Remote.Shell.Interrupt.SSHExecutor.AppLogger;

/// <summary>
/// Implementation of business logic of logging 
/// </summary>
/// <typeparam name="T"></typeparam>
internal class AppLogger<T>(ILoggerFactory loggerFactory) 
    : IAppLogger<T>
{
    readonly ILogger<T> _logger = loggerFactory.CreateLogger<T>()
        ?? throw new ArgumentNullException(nameof(loggerFactory));

    /// <summary>
    /// Write information message log
    /// </summary>
    /// <param name="message">Information message</param>
    /// <param name="args">Additional parameters of information message</param>
    void IAppLogger<T>.LogInformation(string message,
                                      params object[] args)
        => _logger.LogInformation(message,
                                  args);

    /// <summary>
    /// Write warning message log
    /// </summary>
    /// <param name="message">Warning message</param>
    /// <param name="args">Additional parameters of warning message</param>
    void IAppLogger<T>.LogWarning(string message,
                                  params object[] args)
        => _logger.LogWarning(message,
                              args);

    /// <summary>
    /// Write error message log
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="args">Additional parameters of error message</param>
    void IAppLogger<T>.LogError(string message,
                                params object[] args)
        => _logger.LogError(message,
                            args);
}
