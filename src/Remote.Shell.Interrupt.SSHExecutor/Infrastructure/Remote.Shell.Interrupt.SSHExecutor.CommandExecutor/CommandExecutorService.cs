namespace Remote.Shell.Interrupt.SSHExecutor.CommandExecutor;

/// <summary>
/// Dependency Injection of SSH Executor CommandExecutor Service
/// </summary>
public static class CommandExecutorService
{
    /// <summary>
    /// Adding command executing features to the application
    /// </summary>
    /// <param name="services">IServiceCollection services</param>
    /// <param name="configuration">IConfiguration configurations</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddLoggerServices(this IServiceCollection services,
                                                       IConfiguration configuration)
    {
        services.AddScoped<ICommandExecutor, CommandExecutor>();
        return services;
    }
}
