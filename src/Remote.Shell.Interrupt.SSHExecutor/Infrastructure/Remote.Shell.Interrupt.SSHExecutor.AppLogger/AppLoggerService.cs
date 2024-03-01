namespace Remote.Shell.Interrupt.SSHExecutor.AppLogger;

/// <summary>
/// Dependency Injection of SSH Executor AppLogger Service
/// </summary>
public static class AppLoggerService
{
    /// <summary>
    /// Adding logging features to the application
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddLoggerServices(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IAppLogger<>), typeof(AppLogger<>));
        return services;
    }
}
