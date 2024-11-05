namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence;

public static class PersistenceDapperServicesRegistration
{
  public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
                                                          IConfiguration configuration)
  {
    services.AddSingleton<DapperContext>();
    services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    return services;
  }
}
