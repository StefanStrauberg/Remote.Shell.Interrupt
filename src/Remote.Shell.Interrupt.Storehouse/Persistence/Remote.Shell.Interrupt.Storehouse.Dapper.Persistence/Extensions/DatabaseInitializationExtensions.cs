using System.Data.Common;
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
    // Arbitrary but fixed: a Postgres advisory lock is just a 64-bit integer mutex key
    // scoped to the database, with no meaning beyond "the same value = the same lock".
    // Must stay stable across deploys - only its uniqueness within this database matters,
    // not the specific value.
    const long MigrationLockKey = 728_400_913_552_017;

    public static async Task SyncDatabaseAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Generous timeout: first-run deployments create the entire schema.
        dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));

        // Session-scoped Postgres advisory lock, held for the whole method (not
        // pg_advisory_xact_lock, which would only span whichever single transaction
        // MigrateAsync happens to be inside at release time - MigrateAsync can run several).
        // Without it, several replicas of this API starting at once each call MigrateAsync
        // concurrently against the same schema - EF Core migrations aren't designed to run
        // concurrently, so that's a race that can corrupt the schema or deadlock, not just a
        // performance concern. With it, only one replica actually migrates; the others block
        // here, then see __EFMigrationsHistory already at the target version and apply nothing.
        var connection = dbContext.Database.GetDbConnection();
        await connection.OpenAsync(cancellationToken);
        try
        {
            await ExecuteAdvisoryLockCommandAsync(connection, "pg_advisory_lock", cancellationToken);
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
        finally
        {
            await ExecuteAdvisoryLockCommandAsync(connection, "pg_advisory_unlock", cancellationToken);
        }
    }

    static async Task ExecuteAdvisoryLockCommandAsync(DbConnection connection, string function, CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = $"SELECT {function}(@key)";

        var keyParameter = command.CreateParameter();
        keyParameter.ParameterName = "key";
        keyParameter.Value = MigrationLockKey;
        command.Parameters.Add(keyParameter);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
