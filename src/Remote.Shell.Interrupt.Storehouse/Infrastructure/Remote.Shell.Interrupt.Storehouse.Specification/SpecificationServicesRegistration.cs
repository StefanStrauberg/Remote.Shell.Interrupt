namespace Remote.Shell.Interrupt.Storehouse.Specification;

/// <summary>
/// Provides extension methods for registering specification-related services used for filtering and querying entities.
/// </summary>
public static class SpecificationServicesRegistration
{
  /// <summary>
  /// Adds specification and include chain services for various domain entities to the dependency injection container.
  /// </summary>
  /// <param name="services">The service collection to which specification services are added.</param>
  /// <returns>The updated <see cref="IServiceCollection"/> for fluent chaining.</returns>
  public static IServiceCollection AddSpecificationServices(this IServiceCollection services)
  {
    services.AddScoped(typeof(ISpecification<>), typeof(GenericSpecification<>));
    services.AddScoped<IClientSpecification, ClientSpecification>();
    services.AddScoped<IGateSpecification, GateSpecification>();
    services.AddScoped<INetworkDeviceSpecification, NetworkDeviceSpecification>();
    services.AddScoped<ISPRVlanSpecification, SPRVlanSpecification>();
    services.AddScoped<ITfPlanSpecification, TfPlanSpecification>();
    services.AddScoped(typeof(IIncludeChain<>), typeof(IncludeChain<>));
    return services;
  }
}
