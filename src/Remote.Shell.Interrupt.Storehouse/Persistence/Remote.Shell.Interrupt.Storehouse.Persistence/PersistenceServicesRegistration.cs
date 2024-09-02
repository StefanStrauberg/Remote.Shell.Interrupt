namespace Remote.Shell.Interrupt.Storehouse.Persistence;

public static class PersistenceServicesRegistration
{
  public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
                                                          IConfiguration configuration)
  {
    services.AddMarten(options =>
      {
        options.Connection(configuration.GetConnectionString("DefaultConnection")!);
      }).UseLightweightSessions();

    services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    services.AddScoped<IAssignmentRepository, AssignmentRepository>();
    services.AddScoped<IBusinessRuleRepository, BusinessRuleRepository>();
    services.AddScoped<INetworkDeviceRepository, NetworkDeviceRepository>();

    return services;
  }
}
