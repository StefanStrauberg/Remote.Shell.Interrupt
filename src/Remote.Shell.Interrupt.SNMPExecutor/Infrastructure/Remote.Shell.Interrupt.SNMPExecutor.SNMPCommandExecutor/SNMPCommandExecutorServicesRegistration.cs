namespace Remote.Shell.Interrupt.SNMPExecutor.SNMPCommandExecutor;

public static class SNMPCommandExecutorServicesRegistration
{
    public static IServiceCollection AddSNMPCommandExecutorServices(this IServiceCollection services)
    {
        services.AddTransient<ISNMPCommandExecutor, SNMPCommandExecutor>();
        return services;
    }
}
