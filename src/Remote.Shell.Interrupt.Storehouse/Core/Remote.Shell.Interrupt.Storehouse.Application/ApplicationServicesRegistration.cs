namespace Remote.Shell.Interrupt.Storehouse.Application;

public static class ApplicationServicesRegistration
{
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    services.AddScoped(typeof(ISpecification<>), typeof(GenericSpecification<>));
    services.AddScoped<IClientSpecification, ClientSpecification>();
    services.AddScoped<IGateSpecification, GateSpecification>();
    services.AddScoped<INetworkDeviceSpecification, NetworkDeviceSpecification>();
    services.AddScoped<ISPRVlanSpecification, SPRVlanSpecification>();
    services.AddScoped<ITfPlanSpecification, TfPlanSpecification>();
    services.AddScoped<IQueryFilterParser, QueryFilterParser>();

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