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
  // an existing one. Defaults to on (unset = true) so a bare `dotnet run` and the
  // documented docker-compose flow keep working with zero manual setup, in every
  // environment - that default is a deliberate product decision (see README), not an
  // oversight. Database:AutoMigrate=false is the explicit opt-out for a deployment that
  // wants a controlled, manual `dotnet ef database update` step instead (e.g. running
  // migrations once from a release pipeline before scaling up multiple replicas, rather
  // than relying on SyncDatabaseAsync's advisory lock to serialize them all at boot).
  if (app.Configuration.GetValue("Database:AutoMigrate", true))
  {
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
  }
  else
  {
    Log.Warning(
      "Database:AutoMigrate is disabled; skipping automatic EF Core migrations. " +
      "The schema must already be up to date (e.g. via a prior 'dotnet ef database update'), " +
      "or startup will fail once the application queries a table/column a pending migration would add.");
  }

  // Seed identity roles and the default administrator account.
  using (var scope = app.Services.CreateScope())
  {
    await IdentitySeeder.SeedIdentityAsync(scope.ServiceProvider);
  }

  // Seed built-in workflows (e.g. the default network device discovery graph).
  await Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Workflow.WorkflowSeeder.SeedDefaultWorkflowsAsync(app.Services);

  app.Run();
}
// HostAbortedException is how WebApplicationFactory<Program>-style test hosts unwind out of
// Run() once they've captured the built host - not a real startup failure. Excluding it here
// keeps that signal free to propagate to whatever test infrastructure is watching for it,
// rather than being swallowed and misreported as "the host terminated unexpectedly" below.
catch (Exception ex) when (ex is not HostAbortedException)
{
  Log.Fatal("An error occurred during application startup: {Message}", ex.Message);
  Log.Fatal(ex, "Host terminated unexpectedly");

  // Without this, a real startup failure (e.g. Postgres unreachable) still exits the process
  // with code 0: an orchestrator restarting on failed containers, or a deploy script checking
  // the exit code, would see "succeeded" and never know the host never came up.
  Environment.ExitCode = 1;
}
finally
{
  Log.CloseAndFlush();
}

// Exposes the top-level Program as a type integration tests can target (e.g. as the
// TEntryPoint generic argument of WebApplicationFactory<TEntryPoint>, or simply to reference
// its assembly the way Tests.Integration's ApiFactory does to locate controllers).
public partial class Program;
