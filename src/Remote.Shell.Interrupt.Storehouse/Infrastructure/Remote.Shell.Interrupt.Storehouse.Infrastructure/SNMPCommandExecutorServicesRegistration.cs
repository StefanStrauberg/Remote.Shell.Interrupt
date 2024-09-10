using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

namespace Remote.Shell.Interrupt.Storehouse.Infrastructure;

public static class SNMPCommandExecutorServicesRegistration
{
    public static IServiceCollection AddSNMPCommandExecutorServices(this IServiceCollection services)
    {
        services.AddTransient<ISNMPCommandExecutor, SNMPCommandExecutor>();
        return services;
    }
}
