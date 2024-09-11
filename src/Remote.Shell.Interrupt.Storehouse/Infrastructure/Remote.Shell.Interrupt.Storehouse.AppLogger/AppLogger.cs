namespace Remote.Shell.Interrupt.Storehouse.AppLogger;

internal class AppLogger<T>(ILoggerFactory loggerFactory) : IAppLogger<T>
{
    readonly ILogger<T> _logger = loggerFactory.CreateLogger<T>()
        ?? throw new ArgumentNullException(nameof(loggerFactory));

    void IAppLogger<T>.LogInformation(string message, params object[] args)
        => _logger.LogInformation("Message: {Message}. Args: {Args}", message, args);

    void IAppLogger<T>.LogWarning(string message, params object[] args)
        => _logger.LogWarning("Message: {Message}. Args: {Args}", message, args);

    void IAppLogger<T>.LogError(string message, params object[] args)
        => _logger.LogError("Message: {Message}. Args: {Args}", message, args);
}

internal class AppLogger(ILoggerFactory loggerFactory) : IAppLogger
{
    private readonly ILoggerFactory _loggerFactory = loggerFactory
        ?? throw new ArgumentNullException(nameof(loggerFactory));

    public void LogInformation(string className, string message, params object[] args)
    {
        var logger = _loggerFactory.CreateLogger(className);
        logger.LogInformation("Class: {ClassName}, Message: {Message}", className, string.Format(message, args));
    }

    public void LogWarning(string className, string message, params object[] args)
    {
        var logger = _loggerFactory.CreateLogger(className);
        logger.LogWarning("Class: {ClassName}, Message: {Message}", className, string.Format(message, args));
    }

    public void LogError(string className, string message, params object[] args)
    {
        var logger = _loggerFactory.CreateLogger(className);
        logger.LogError("Class: {ClassName}, Message: {Message}", className, string.Format(message, args));
    }
}