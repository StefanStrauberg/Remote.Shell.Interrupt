namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence;

/// <summary>
/// Provides extension methods for registering persistence-layer services including
/// repositories, contexts, and unit of work abstractions.
/// </summary>
public static class PersistenceServicesRegistration
{
  /// <summary>
  /// Registers all data access services, including EF Core and Dapper contexts,
  /// repositories for CRUD and bulk operations, and unit of work abstractions.
  /// </summary>
  /// <param name="services">The DI service collection used during application startup.</param>
  /// <param name="configuration">The application configuration instance for connection string resolution.</param>
  /// <returns>The updated <see cref="IServiceCollection"/> for fluent chaining.</returns>
  public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
                                                          IConfiguration configuration)
  {
    // Database contexts
    services.AddScoped<MySQLDapperContext>();
    services.AddDbContext<ApplicationDbContext>(optionsBuilder =>
    {
      optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
    });

    // Generic query repositories
    services.AddScoped(typeof(ICountRepository<>), typeof(CountRepository<>));
    services.AddScoped(typeof(IExistenceQueryRepository<>), typeof(ExistenceQueryRepository<>));
    services.AddScoped(typeof(IManyQueryRepository<>), typeof(ManyQueryRepository<>));
    services.AddScoped(typeof(IOneQueryRepository<>), typeof(OneQueryRepository<>));
    services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));

    // Generic mutation repositories
    services.AddScoped(typeof(IDeleteRepository<>), typeof(DeleteRepository<>));
    services.AddScoped(typeof(IInsertRepository<>), typeof(InsertRepository<>));
    services.AddScoped(typeof(IReplaceRepository<>), typeof(ReplaceRepository<>));

    // Bulk mutation repositories
    services.AddScoped(typeof(IBulkDeleteRepository<>), typeof(BulkDeleteRepository<>));
    services.AddScoped(typeof(IBulkInsertRepository<>), typeof(BulkInsertRepository<>));
    services.AddScoped(typeof(IBulkReplaceRepository<>), typeof(BulkReplaceRepository<>));

    // Remote data repositories
    services.AddScoped<IRemoteClientsRepository, RemoteClientsRepository>();
    services.AddScoped<IRemoteCODRepository, RemoteCODRepository>();
    services.AddScoped<IRemoteSPRVlansRepository, RemoteSPRVlansRepository>();
    services.AddScoped<IRemoteTfPlanRepository, RemoteTfPlanRepository>();

    // Entity-specific repositories
    services.AddScoped<IGateRepository, GateRepository>();
    services.AddScoped<IClientsRepository, ClientsRepository>();
    services.AddScoped<ISPRVlansRepository, SPRVlansRepository>();
    services.AddScoped<ITfPlanRepository, TfPlanRepository>();
    services.AddScoped<ICODRepository, CODRepository>();
    services.AddScoped<INetworkDeviceRepository, NetworkDeviceRepository>();
    services.AddScoped<IARPEntityRepository, ARPEntityRepository>();
    services.AddScoped<IMACEntityRepository, MACEntityRepository>();
    services.AddScoped<IVLANRepository, VLANRepository>();
    services.AddScoped<ITerminatedNetworkEntityRepository, TerminatedNetworkEntityRepository>();

    // Unit of work abstractions
    services.AddScoped<IGateUnitOfWork, GateUnitOfWork>();
    services.AddScoped<ILocBillUnitOfWork, LocBillUnitOfWork>();
    services.AddScoped<INetDevUnitOfWork, NetDevUnitOfWork>();
    services.AddScoped<IRemBillUnitOfWork, RemBillUnitOfWork>();

    return services;
  }
}
