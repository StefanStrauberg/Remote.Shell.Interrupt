namespace Remote.Shell.Interrupt.SNMPExecutor.Application;

public static class ApplicationService
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(src =>
        {
            src.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient<ExceptionHandlingMiddleware>();
        return services;
    }
}
