namespace Remote.Shell.Interrupt.Storehouse.API;

/// <summary>
/// Provides extension methods for registering services and configuring middleware in the application pipeline.
/// </summary>
public static class ServiceRegistration
{
  /// <summary>
  /// Configures dependency injection for core application components, infrastructure services, and cross-cutting concerns.
  /// </summary>
  /// <param name="builder">The application builder used to register services during startup.</param>
  public static void AddApplicationServices(this WebApplicationBuilder builder)
  {
    // Logging
    builder.Services.AddLoggerServices();
    builder.Services.AddSingleton(sp => new LoggerFactory().AddSerilog(Log.Logger));

    // Application Layers
    builder.Services.AddApplicationServices();
    builder.Services.AddSNMPCommandExecutorServices();
    builder.Services.AddSpecificationServices();
    builder.Services.AddQueryFilterParserServices();
    builder.Services.AddPersistenceServices(builder.Configuration);

    // API Infrastructure
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Cross-cutting concerns
    builder.Services.AddCors(options =>
                    {
                      options.AddPolicy(DefaultEntities.CorsPolicyName,
                                        builder => builder.AllowAnyOrigin()
                                                          .AllowAnyHeader()
                                                          .AllowAnyMethod()
                                                          .WithExposedHeaders(DefaultEntities.ExposedHeaders));
                    });
  }

  /// <summary>
  /// Configures middleware components for the application, including CORS, Swagger UI, and global exception handling.
  /// </summary>
  /// <param name="app">The web application instance to configure.</param>
  public static void ConfigurePipeline(this WebApplication app)
  {
    app.UseCors(DefaultEntities.CorsPolicyName);

    // Development-specific middleware
    app.UseSwagger();
    app.UseSwaggerUI();

    // Application middleware
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.MapControllers();
  }
}
