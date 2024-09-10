Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

try
{
  var builder = WebApplication.CreateBuilder(args);

  // Register Services
  builder.RegisterServices();

  var app = builder.Build();

  // Register Middlewares
  app.RegisterMiddlewares();

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
