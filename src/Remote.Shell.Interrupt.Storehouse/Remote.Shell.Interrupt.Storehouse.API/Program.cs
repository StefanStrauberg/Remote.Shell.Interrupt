using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Identity;

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

  // Seed identity roles and the default administrator account.
  using (var scope = app.Services.CreateScope())
  {
    await IdentitySeeder.SeedIdentityAsync(scope.ServiceProvider);
  }

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
