namespace Remote.Shell.Interrupt.SNMPExecutor.SNMPCommandExecutor;

public static class SNMPCommandExecutorService
{
    /// <summary>
    /// Adding command executing features to the application
    /// </summary>
    /// <param name="services">IServiceCollection services</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddSNMPCommandExecutorServices(this IServiceCollection services)
    {
        services.AddTransient<ISNMPCommandExecutor, SNMPCommandExecutor>();
        return services;
    }
}
