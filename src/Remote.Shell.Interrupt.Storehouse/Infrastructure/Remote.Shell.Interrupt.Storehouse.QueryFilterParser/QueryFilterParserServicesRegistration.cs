namespace Remote.Shell.Interrupt.Storehouse.QueryFilterParser;

public static class QueryFilterParserServicesRegistration
{
  public static IServiceCollection AddQueryFilterParserServices(this IServiceCollection services)
  {
    services.AddScoped<IQueryFilterParser, CommonQueryFilterParser>();
    return services;
  }
}
