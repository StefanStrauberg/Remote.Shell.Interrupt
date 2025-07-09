namespace Remote.Shell.Interrupt.Storehouse.Infrastructure.SNMPCommandExecutor;

/// <summary>
/// Provides extension methods for registering SNMP command executor services.
/// </summary>
public static class SNMPCommandExecutorServicesRegistration
{
    /// <summary>
    /// Registers the <see cref="ISNMPCommandExecutor"/> implementation used for executing SNMP commands.
    /// </summary>
    /// <param name="services">The service collection to which SNMP services are added.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> for fluent chaining.</returns>
    public static IServiceCollection AddSNMPCommandExecutorServices(this IServiceCollection services)
    {
        services.AddTransient<ISNMPCommandExecutor, SNMPCommandExecutor>();
        return services;
    }
}
