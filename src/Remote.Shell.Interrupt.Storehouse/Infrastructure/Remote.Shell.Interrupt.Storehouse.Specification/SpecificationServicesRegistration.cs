namespace Remote.Shell.Interrupt.Storehouse.Specification;

public static class SpecificationServicesRegistration
{
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
