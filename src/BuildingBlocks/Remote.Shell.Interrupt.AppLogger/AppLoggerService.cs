namespace Remote.Shell.Interrupt.AppLogger;

public static class AppLoggerService
{
    public static IServiceCollection AddLoggerServices(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IAppLogger<>), typeof(AppLogger<>));
        return services;
    }
}
