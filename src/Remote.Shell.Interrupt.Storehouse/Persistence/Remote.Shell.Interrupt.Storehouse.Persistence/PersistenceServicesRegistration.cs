namespace Remote.Shell.Interrupt.Storehouse.Persistence;

public static class PersistenceServicesRegistration
{
  public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
                                                          IConfiguration configuration)
  {
    services.AddDbContext<ApplicationDbContext>(options =>
    {
      options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
    });

    services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    services.AddScoped<IAssignmentRepository, AssignmentRepository>();
    services.AddScoped<IBusinessRuleRepository, BusinessRuleRepository>();
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    services.AddScoped<IVLANRepository, VLANRepository>();
    services.AddScoped<IUnitOfWork, UnitOfWork>();

    return services;
  }
}
