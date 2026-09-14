using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Identity;

const string outputTemplate =
  "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration().Filter.ByExcluding(e => e.Exception is HostAbortedException)
                                      .MinimumLevel.Information()
                                      .Enrich.FromLogContext()
                                      .WriteTo.Console(outputTemplate: outputTemplate)
                                      .WriteTo.File(DefaultEntities.LoggingTo, rollingInterval: RollingInterval.Day, outputTemplate: outputTemplate)
                                      .CreateLogger();

try
{
  var builder = WebApplication.CreateBuilder(args);

  // Register Services
  builder.AddApplicationServices();

  var app = builder.Build();

  // Register Middlewares
  app.ConfigurePipeline();

  // Applies pending EF Core migrations: creates the full schema on a fresh/empty
  // database (e.g. a newly deployed container) and applies only what's new on
  // an existing one.
  try
  {
    await app.Services.SyncDatabaseAsync();
    Log.Information("Database migration completed successfully.");
  }
  catch (Exception ex)
  {
    Log.Fatal(ex,
      "Database schema synchronization failed. Verify the 'DefaultConnection' string " +
      "and that the PostgreSQL server is reachable, then restart the host.");
    throw;
  }

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
