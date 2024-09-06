namespace Remote.Shell.Interrupt.Storehouse.Application;

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

    // AutoMapper
    services.AddAutoMapper(cfg =>
    {
      cfg.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
    });

    // Exception Handling Middleware injection
    services.AddTransient<ExceptionHandlingMiddleware>();

    // FluentValidation injection
    services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

    return services;
  }
}
