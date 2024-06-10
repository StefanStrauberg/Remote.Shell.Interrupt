namespace Remote.Shell.Interrupt.AppLogger;

public static class AppLoggerService
{
    public static IServiceCollection AddLoggerServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));
        return services;
    }
}
