namespace Remote.Shell.Interrupt.SNMPExecutor.Presentation;

public static class PresentationServicesRegistration
{
  public static IServiceCollection AddPresentationServicesServices(this IServiceCollection services)
  {
    services.AddCarter();
    services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
    return services;
  }
}
