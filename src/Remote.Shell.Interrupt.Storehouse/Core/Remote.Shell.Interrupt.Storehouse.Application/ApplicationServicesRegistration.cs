namespace Remote.Shell.Interrupt.Storehouse.Application;

/// <summary>
/// Provides extension methods to register core application services including Mapster, validation, and middleware.
/// </summary>
public static class ApplicationServicesRegistration
{
  /// <summary>
  /// Adds essential application services to the dependency injection container.
  /// This includes Mapster mapping registrations, FluentValidation, and exception handling middleware.
  /// Mediator registration itself lives in the API project's <c>ServiceRegistration.AddApplicationServices</c>
  /// (see its "Application Layers" section) - the source generator that implements <c>AddMediator</c> only
  /// runs in the project that references <c>Mediator.SourceGenerator</c> (the API/edge project), so the
  /// generated method isn't available to call from here.
  /// </summary>
  /// <param name="services">The service collection to which dependencies are registered.</param>
  /// <returns>The updated <see cref="IServiceCollection"/> for fluent chaining.</returns>
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    services.AddTransient<FindEntitiesByFilterQueryHandler<SPRVlan, SPRVlanDTO, GetSPRVlansByFilterQuery>, GetSPRVlansByFilterQueryHandler>();
    services.AddTransient<CQRS.IQueryHandler<GetClientsByVlanTagQuery, IEnumerable<DetailClientDTO>>, GetClientsByVlanTagQueryHandler>();

    // Mapster injection - a fresh (non-global) config instance, not TypeAdapterConfig.GlobalSettings,
    // so registrations don't leak across parallel test runs via static state.
    var mapperConfig = new TypeAdapterConfig();
    mapperConfig.Scan(Assembly.GetExecutingAssembly());
    services.AddSingleton(mapperConfig);
    services.AddScoped<IMapper, Mapper>();

    // Exception Handling Middleware injection
    services.AddScoped<ExceptionHandlingMiddleware>();

    // FluentValidation injection
    services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

    return services;
  }
}