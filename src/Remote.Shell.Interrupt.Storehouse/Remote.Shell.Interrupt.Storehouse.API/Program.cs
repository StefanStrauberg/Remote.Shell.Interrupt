Log.Logger = new LoggerConfiguration().MinimumLevel.Verbose()
                                      .WriteTo.Console()
                                      .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                                      .CreateLogger();

try
{
  Log.Information("Starting application initialization...");
  var builder = WebApplication.CreateBuilder(args);

  // Register Services
  builder.RegisterServices();

  Log.Information("Building application...");
  var app = builder.Build();

  // Register Middlewares
  Log.Information("Registering middlewares...");
  app.RegisterMiddlewares();

  Log.Information("Running application...");
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
