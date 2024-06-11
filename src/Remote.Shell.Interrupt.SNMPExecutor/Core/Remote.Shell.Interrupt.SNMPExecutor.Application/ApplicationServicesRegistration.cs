namespace Remote.Shell.Interrupt.SNMPExecutor.Application;

public static class ApplicationServicesRegistration
{
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    services.AddMediatR(cfg =>
    {
      cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
    });
    return services;
  }
}
