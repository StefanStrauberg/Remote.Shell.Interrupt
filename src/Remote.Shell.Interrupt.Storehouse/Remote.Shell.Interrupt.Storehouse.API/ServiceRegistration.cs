namespace Remote.Shell.Interrupt.Storehouse.API;

public static class ServiceRegistration
{
  public static void AddApplicationServices(this WebApplicationBuilder builder)
  {
    // Logging
    builder.Services.AddLoggerServices();
    builder.Services.AddSingleton<ILoggerFactory>(sp => new LoggerFactory().AddSerilog(Log.Logger));

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
                      options.AddPolicy("CorsPolicy",
                                            builder => builder.AllowAnyOrigin()
                                                              .AllowAnyHeader()
                                                              .AllowAnyMethod()
                                                              .WithExposedHeaders("X-Pagination"));
                    });
    Log.Debug("Dependency injection registration completed.");
  }

  public static void ConfigurePipeline(this WebApplication app)
  {
    app.UseCors("CorsPolicy");

    // Development-specific middleware
    app.UseSwagger();
    app.UseSwaggerUI();

    // Application middleware
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.MapControllers();
  }
}
