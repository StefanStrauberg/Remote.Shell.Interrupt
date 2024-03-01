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
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddCommandExecutorServices(this IServiceCollection services)
    {
        services.AddTransient<ICommandExecutor, CommandExecutor>();
        return services;
    }
}
