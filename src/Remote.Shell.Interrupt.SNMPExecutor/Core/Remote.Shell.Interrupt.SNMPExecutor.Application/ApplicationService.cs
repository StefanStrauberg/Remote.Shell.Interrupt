namespace Remote.Shell.Interrupt.SNMPExecutor.Application;

/// <summary>
/// Dependency Injection of SSH Executor Application Service
/// </summary>
public static class ApplicationService
{
    /// <summary>
    /// Injection Application Services
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <returns></returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(src =>
        {
            src.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        return services;
    }
}
