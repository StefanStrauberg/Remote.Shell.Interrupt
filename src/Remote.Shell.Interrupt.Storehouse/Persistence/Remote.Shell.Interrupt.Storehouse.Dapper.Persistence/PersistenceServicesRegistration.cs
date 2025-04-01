namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence;

public static class PersistenceServicesRegistration
{
  public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
                                                          IConfiguration configuration)
  {
    services.AddScoped<PostgreSQLDapperContext>();
    services.AddScoped<MySQLDapperContext>();
    services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    services.AddScoped<IVLANRepository, VLANRepository>();
    services.AddScoped<IPortRepository, PortRepository>();
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    services.AddScoped<IRemoteClientsRepository, RemoteClientsRepository>();
    services.AddScoped<IRemoteCODRepository, RemoteCODRepository>();
    services.AddScoped<IRemoteTfPlanRepository, RemoteTfPlanRepository>();
    services.AddScoped<IClientsRepository, ClientsRepository>();
    services.AddScoped<IGateRepository, GateRepository>();

    return services;
  }
}
