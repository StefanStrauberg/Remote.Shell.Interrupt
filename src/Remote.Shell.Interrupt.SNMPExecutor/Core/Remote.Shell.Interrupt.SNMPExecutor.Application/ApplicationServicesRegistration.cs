namespace Remote.Shell.Interrupt.SNMPExecutor.Application;

public static class ApplicationServicesRegistration
{
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    services.AddMediatR(config =>
    {
      config.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
      config.AddOpenBehavior(typeof(ValidationBehavior<,>));
      config.AddOpenBehavior(typeof(LoggingBehavior<,>));
    });
    services.AddTransient<ExceptionHandlingMiddleware>();
    return services;
  }
}
