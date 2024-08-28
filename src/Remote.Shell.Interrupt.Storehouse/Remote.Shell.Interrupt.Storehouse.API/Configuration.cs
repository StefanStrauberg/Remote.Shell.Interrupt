namespace Remote.Shell.Interrupt.Storehouse.API;

public static class Configuration
{
  public static void RegisterServices(this WebApplicationBuilder builder)
  {
    builder.Services.AddLoggerServices();
    builder.Services.AddApplicationServices();
    builder.Services.AddSNMPCommandExecutorServices();
    builder.Services.AddPersistenceServices(builder.Configuration);
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddCors(options =>
                    {
                      options.AddPolicy("CorsPolicy",
                                            builder => builder.AllowAnyOrigin()
                                                              .AllowAnyHeader()
                                                              .AllowAnyMethod());
                    });
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
