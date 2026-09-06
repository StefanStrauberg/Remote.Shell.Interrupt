using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Identity;

Log.Logger = new LoggerConfiguration().Filter.ByExcluding(e => e.Exception is HostAbortedException)
                                      .MinimumLevel.Information()
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

  // Idempotent schema sync: executes the embedded idempotent SQL script that
  // creates missing tables/indices/constraints, skips everything that already
  // exists, and stamps "__EFMigrationsHistory" so future migrations stay
  // consistent with the deployed schema.
  try
  {
    await app.Services.SyncDatabaseAsync();
    Log.Information("Idempotent database schema sync completed successfully.");
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
