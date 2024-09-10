using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

namespace Remote.Shell.Interrupt.Storehouse.Persistence;

public static class PersistenceServicesRegistration
{
  public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
                                                          IConfiguration configuration)
  {
    services.AddDbContext<ApplicationDbContext>(options =>
    {
      options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
      options.LogTo(Console.WriteLine);
    });

    services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    services.AddScoped<IAssignmentRepository, AssignmentRepository>();
    services.AddScoped<IBusinessRuleRepository, BusinessRuleRepository>();
    services.AddScoped<INetworkDeviceRepository, NetworkDeviceRepository>();

    return services;
  }
}
