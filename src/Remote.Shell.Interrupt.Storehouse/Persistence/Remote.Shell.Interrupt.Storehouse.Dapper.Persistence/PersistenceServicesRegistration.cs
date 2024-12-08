namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence;

public static class PersistenceServicesRegistration
{
  public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
                                                          IConfiguration configuration)
  {
    services.AddSingleton<DapperContext>();
    services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    services.AddScoped<IAssignmentRepository, AssignmentRepository>();
    services.AddScoped<IBusinessRuleRepository, BusinessRuleRepository>();
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    services.AddScoped<IVLANRepository, VLANRepository>();
    services.AddScoped<IPortRepository, PortRepository>();
    services.AddScoped<IUnitOfWork, UnitOfWork>();

    return services;
  }
}
