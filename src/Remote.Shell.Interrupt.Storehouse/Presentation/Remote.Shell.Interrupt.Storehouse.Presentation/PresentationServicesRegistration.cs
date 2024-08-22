namespace Remote.Shell.Interrupt.Storehouse.Presentation;

public static class PresentationServicesRegistration
{
  public static IServiceCollection AddPresentationServicesServices(this IServiceCollection services)
  {
    services.AddCarter();
    TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

    return services;
  }
}
