namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence;

public static class PersistenceServicesRegistration
{
  public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
                                                          IConfiguration configuration)
  {
    services.AddScoped<MySQLDapperContext>();

    services.AddDbContext<ApplicationDbContext>(optionsBuilder => 
    {
      optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
    });

    services.AddScoped(typeof(ICountRepository<>), typeof(CountRepository<>));

    services.AddScoped(typeof(IExistenceQueryRepository<>), typeof(ExistenceQueryRepository<>));

    services.AddScoped(typeof(IManyQueryRepository<>), typeof(ManyQueryRepository<>));
    services.AddScoped(typeof(IOneQueryRepository<>), typeof(OneQueryRepository<>));

    services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));

    services.AddScoped(typeof(IDeleteRepository<>),typeof(DeleteRepository<>));
    services.AddScoped(typeof(IInsertRepository<>),typeof(InsertRepository<>));
    services.AddScoped(typeof(IReplaceRepository<>),typeof(ReplaceRepository<>));

    services.AddScoped(typeof(IBulkDeleteRepository<>),typeof(BulkDeleteRepository<>));
    services.AddScoped(typeof(IBulkInsertRepository<>),typeof(BulkInsertRepository<>));
    services.AddScoped(typeof(IBulkReplaceRepository<>),typeof(BulkReplaceRepository<>));

    services.AddScoped<IRemoteClientsRepository, RemoteClientsRepository>();
    services.AddScoped<IRemoteCODRepository, RemoteCODRepository>();
    services.AddScoped<IRemoteSPRVlansRepository, RemoteSPRVlansRepository>();
    services.AddScoped<IRemoteTfPlanRepository, RemoteTfPlanRepository>();

    services.AddScoped<IGateRepository, GateRepository>();

    services.AddScoped<IClientsRepository, ClientsRepository>();
    services.AddScoped<ISPRVlansRepository, SPRVlansRepository>();
    services.AddScoped<ITfPlanRepository, TfPlanRepository>();
    services.AddScoped<ICODRepository, CODRepository>();

    services.AddScoped<INetworkDeviceRepository, NetworkDeviceRepository>();
    services.AddScoped<IARPEntityRepository, ARPEntityRepository>();
    services.AddScoped<IMACEntityRepository, MACEntityRepository>();
    services.AddScoped<IVLANRepository, VLANRepository>();
    services.AddScoped<IPortVlanRepository, PortVlanRepository>();
    services.AddScoped<ITerminatedNetworkEntityRepository, TerminatedNetworkEntityRepository>();

    services.AddScoped<IGateUnitOfWork, GateUnitOfWork>();
    services.AddScoped<ILocBillUnitOfWork, LocBillUnitOfWork>();
    services.AddScoped<INetDevUnitOfWork, NetDevUnitOfWork>();
    services.AddScoped<IRemBillUnitOfWork, RemBillUnitOfWork>();

    return services;
  }
}
