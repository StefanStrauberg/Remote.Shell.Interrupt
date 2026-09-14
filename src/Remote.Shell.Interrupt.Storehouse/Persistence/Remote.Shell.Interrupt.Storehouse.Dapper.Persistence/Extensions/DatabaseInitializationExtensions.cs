using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence;

/// <summary>
/// Startup database initialization.
///
/// Strategy: real EF Core migrations, applied via <see cref="DbContext.Database"/>'s
/// <c>MigrateAsync</c>. On a fresh/empty PostgreSQL database (e.g. a newly deployed
/// container) this creates the full schema from the migration history; on an existing
/// database it applies only the migrations not yet recorded in
/// "__EFMigrationsHistory". Author schema changes the normal EF Core way:
///
///   dotnet ef migrations add &lt;Name&gt; \
///     --project src/.../Remote.Shell.Interrupt.Storehouse.Dapper.Persistence \
///     --startup-project src/.../Remote.Shell.Interrupt.Storehouse.Dapper.Persistence \
///     --output-dir Migrations
///
/// (The Persistence project doubles as the startup project via
/// <see cref="Configuration.ApplicationDbContextFactory"/>, so this works without
/// building the API host or supplying its runtime secrets.) No manual step is needed
/// to deploy the result: this method picks up and applies any new migration on the
/// next application start. This only covers PostgreSQL ("DefaultConnection"); the
/// MySQL side (<see cref="Context.MySQLDapperContext"/>) is a read-only mirror of an
/// externally-owned database and is never migrated by this application.
/// </summary>
public static class DatabaseInitializationExtensions
{
    public static async Task SyncDatabaseAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Generous timeout: first-run deployments create the entire schema.
        dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));

        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
