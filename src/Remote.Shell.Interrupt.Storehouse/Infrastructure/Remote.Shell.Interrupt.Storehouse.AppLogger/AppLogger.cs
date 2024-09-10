namespace Remote.Shell.Interrupt.Storehouse.AppLogger;

internal class AppLogger<T>(ILoggerFactory loggerFactory) : IAppLogger<T>
{
    readonly ILogger<T> _logger = loggerFactory.CreateLogger<T>()
        ?? throw new ArgumentNullException(nameof(loggerFactory));

    void IAppLogger<T>.LogInformation(string message, params object[] args)
        => _logger.LogInformation(message, args);

    void IAppLogger<T>.LogWarning(string message, params object[] args)
        => _logger.LogWarning(message, args);

    void IAppLogger<T>.LogError(string message, params object[] args)
        => _logger.LogError(message, args);
}
