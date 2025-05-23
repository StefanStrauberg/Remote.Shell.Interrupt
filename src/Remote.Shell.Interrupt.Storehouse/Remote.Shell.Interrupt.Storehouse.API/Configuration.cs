namespace Remote.Shell.Interrupt.Storehouse.API;

public static class Configuration
{
  public static void RegisterServices(this WebApplicationBuilder builder)
  {
    Log.Debug("Starting dependency injection registration...");
    builder.Services.AddLoggerServices();
    builder.Services.AddSingleton<ILoggerFactory>(sp => new LoggerFactory().AddSerilog(Log.Logger));
    builder.Services.AddApplicationServices();
    builder.Services.AddSNMPCommandExecutorServices();
    builder.Services.AddSpecificationServices();
    builder.Services.AddQueryFilterParserServices();
    builder.Services.AddPersistenceServices(builder.Configuration);
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
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

  public static void RegisterMiddlewares(this WebApplication app)
  {
    app.UseCors("CorsPolicy");
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.MapControllers();
  }
}
