namespace Remote.Shell.Interrupt.Storehouse.AppLogger;

/// <summary>
/// Provides extension methods for registering application-level logging services.
/// </summary>
public static class AppLoggerServicesRegistration
{
    /// <summary>
    /// Registers scoped implementations of <see cref="IAppLogger{T}"/> and <see cref="IAppLogger"/> for dependency injection.
    /// </summary>
    /// <param name="services">The service collection to which logging services are added.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> for further configuration chaining.</returns>
    public static IServiceCollection AddLoggerServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));
        services.AddScoped<IAppLogger, AppLogger>();
        return services;
    }
}
