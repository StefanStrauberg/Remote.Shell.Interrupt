Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
                                      .Enrich.FromLogContext()
                                      .WriteTo.Console()
                                      .WriteTo.File(DefaultEntities.LoggingTo, rollingInterval: RollingInterval.Day)
                                      .CreateLogger();

try
{
  var builder = WebApplication.CreateBuilder(args);

  // Register Services
  builder.AddApplicationServices();

  var app = builder.Build();

  // Register Middlewares
  app.ConfigurePipeline();

  app.Run();
}
catch (Exception ex)
{
  Log.Fatal("An error occurred during application startup: {Message}", ex.Message);
  Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
  Log.CloseAndFlush();
}
