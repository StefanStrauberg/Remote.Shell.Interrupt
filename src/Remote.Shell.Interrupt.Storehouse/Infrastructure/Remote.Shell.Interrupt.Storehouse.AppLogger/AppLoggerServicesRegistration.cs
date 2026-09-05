namespace Remote.Shell.Interrupt.Storehouse.AppLogger;

/// <summary>
/// Provides extension methods for registering application-level logging services.
/// </summary>
public static class AppLoggerServicesRegistration
{
    /// <summary>
    /// Registers singleton implementations of <see cref="IAppLogger{T}"/> and <see cref="IAppLogger"/> for dependency injection.
    /// Loggers are stateless wrappers around <see cref="ILoggerFactory"/>, so a single instance is sufficient.
    /// </summary>
    /// <param name="services">The service collection to which logging services are added.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> for further configuration chaining.</returns>
    public static IServiceCollection AddLoggerServices(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IAppLogger<>), typeof(AppLogger<>));
        services.AddSingleton<IAppLogger, AppLogger>();
        return services;
    }
}
