namespace Remote.Shell.Interrupt.SNMPExecutor.Application;

public static class ApplicationServicesRegistration
{
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    // MediatR injection
    services.AddMediatR(config =>
    {
      config.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
      config.AddOpenBehavior(typeof(ValidationBehavior<,>));
      config.AddOpenBehavior(typeof(LoggingBehavior<,>));
    });

    // Exception Handling Middleware injection
    services.AddTransient<ExceptionHandlingMiddleware>();

    // FluentValidation injection
    services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

    // Mapster injection
    TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

    return services;
  }
}
