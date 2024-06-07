namespace Remote.Shell.Interrupt.SNMPExecutor.SNMPCommandExecutor;

public static class SNMPCommandExecutorService
{
    public static IServiceCollection AddSNMPCommandExecutorServices(this IServiceCollection services)
    {
        services.AddTransient<ISNMPCommandExecutor, SNMPCommandExecutor>();
        return services;
    }
}
