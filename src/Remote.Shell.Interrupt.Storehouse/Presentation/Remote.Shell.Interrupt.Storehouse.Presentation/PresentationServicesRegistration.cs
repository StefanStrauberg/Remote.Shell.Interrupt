namespace Remote.Shell.Interrupt.Storehouse.Presentation;

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
