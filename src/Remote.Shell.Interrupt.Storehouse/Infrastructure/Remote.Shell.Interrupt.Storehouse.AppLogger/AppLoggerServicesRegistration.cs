namespace Remote.Shell.Interrupt.Storehouse.AppLogger;

public static class AppLoggerServicesRegistration
{
    public static IServiceCollection AddLoggerServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));
        return services;
    }
}
