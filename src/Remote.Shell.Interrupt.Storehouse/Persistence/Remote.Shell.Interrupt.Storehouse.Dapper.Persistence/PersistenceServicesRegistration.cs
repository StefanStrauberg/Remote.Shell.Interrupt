namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence;

public static class PersistenceServicesRegistration
{
  public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
                                                          IConfiguration configuration)
  {
    services.AddScoped<PostgreSQLDapperContext>();
    services.AddScoped<MySQLDapperContext>();
    services.AddScoped(typeof(ICountRepository<>), typeof(CountRepository<>));
    services.AddScoped(typeof(IExistenceQueryRepository<>), typeof(ExistenceQueryRepository<>));
    services.AddScoped(typeof(IManyQueryRepository<>), typeof(ManyQueryRepository<>));
    services.AddScoped(typeof(IOneQueryRepository<>), typeof(OneQueryRepository<>));
    services.AddScoped(typeof(IWriteOneRepository<>), typeof(WriteOneRepository<>));
    services.AddScoped<IGateRepository, GateRepository>();
    services.AddScoped<IClientsRepository, ClientsRepository>();
    services.AddScoped<ISPRVlansRepository, SPRVlansRepository>();
    services.AddScoped<ITfPlanRepository, TfPlanRepository>();

    return services;
  }
}
