namespace Remote.Shell.Interrupt.Storehouse.Application;

/// <summary>
/// Provides extension methods to register core application services including MediatR, AutoMapper, validation, and middleware.
/// </summary>
public static class ApplicationServicesRegistration
{
  /// <summary>
  /// Adds essential application services to the dependency injection container.
  /// This includes MediatR behaviors, AutoMapper profiles, FluentValidation, and exception handling middleware.
  /// </summary>
  /// <param name="services">The service collection to which dependencies are registered.</param>
  /// <returns>The updated <see cref="IServiceCollection"/> for fluent chaining.</returns>
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    // MediatR injection
    services.AddMediatR(config =>
    {
      config.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
      config.AddOpenBehavior(typeof(ValidationBehavior<,>));
      config.AddOpenBehavior(typeof(LoggingBehavior<,>));
    });

    // AutoMapper injection
    services.AddAutoMapper(cfg =>
    {
      cfg.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
    });

    // Exception Handling Middleware injection
    services.AddScoped<ExceptionHandlingMiddleware>();

    // FluentValidation injection
    services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

    return services;
  }
}