using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence;

/// <summary>
/// Startup database initialization.
///
/// Strategy: an idempotent SQL script (generated with
/// <c>dotnet ef migrations script --idempotent</c>, embedded as a resource)
/// is executed raw against the target database. PostgreSQL natively skips
/// every object that already exists, so a database built before EF migrations
/// (partial or hand-created tables, no "__EFMigrationsHistory") is brought to
/// the full current schema — including the Identity tables — in a single
/// transactional pass, without any 'relation already exists' failures.
/// </summary>
public static class DatabaseInitializationExtensions
{
    const string IdempotentScriptResourceName = "InitialDatabaseSync.sql";

    const string InitialMigrationId = "20260906215217_InitialIdentitySetup";
    const string EfProductVersion = "9.0.4";

    /// <summary>
    /// Synchronizes the target database with the current EF Core model:
    /// executes the embedded idempotent script (skips existing objects,
    /// deploys missing ones) and stamps "__EFMigrationsHistory" with the
    /// initial migration id when absent, so future EF migrations treat the
    /// database as baseline-synced instead of conflicting.
    /// </summary>
    public static async Task SyncDatabaseAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var script = HardenIdempotentScript(ReadEmbeddedIdempotentScript());

        // Generous timeout: first-run deployments create the entire schema.
        dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));

        // Single transactional pass: the script opens its own
        // START TRANSACTION / COMMIT block.
        await dbContext.Database.ExecuteSqlRawAsync(script, cancellationToken);

        await ReconcileMigrationsHistoryAsync(dbContext, cancellationToken);
    }

    /// <summary>
    /// Reads the EF-generated idempotent script embedded in this assembly.
    /// </summary>
    static string ReadEmbeddedIdempotentScript()
    {
        var assembly = typeof(ApplicationDbContext).Assembly;

        using var stream = assembly.GetManifestResourceStream(IdempotentScriptResourceName)
            ?? throw new InvalidOperationException(
                $"Embedded idempotent SQL script '{IdempotentScriptResourceName}' was not found in " +
                $"'{assembly.GetName().Name}'. Regenerate it with: " +
                "dotnet ef migrations script --idempotent -o Scripts/InitialDatabaseSync.sql " +
                "--project <persistence> --startup-project <api>");

        using var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Hardens the EF-generated script for databases that predate migrations.
    ///
    /// EF's "--idempotent" output only guards each operation group by
    /// "__EFMigrationsHistory" — on a database whose tables exist without any
    /// history record the guarded body would still fail with
    /// 'relation already exists'. EF emits plain "CREATE TABLE"/"CREATE INDEX"
    /// for the migration body and machine-generated SQL has no other
    /// statement shapes starting with these prefixes, so upgrading them to
    /// "IF NOT EXISTS" makes every DDL statement a no-op on existing objects.
    /// Foreign keys are declared inline in the CREATE TABLE statements and are
    /// skipped together with their tables.
    /// </summary>
    static string HardenIdempotentScript(string script)
        => script
            .Replace("CREATE TABLE \"", "CREATE TABLE IF NOT EXISTS \"", StringComparison.Ordinal)
            .Replace("CREATE INDEX \"", "CREATE INDEX IF NOT EXISTS \"", StringComparison.Ordinal)
            .Replace("CREATE UNIQUE INDEX \"", "CREATE UNIQUE INDEX IF NOT EXISTS \"", StringComparison.Ordinal);

    /// <summary>
    /// Guarantees "__EFMigrationsHistory" exists and carries the initial
    /// migration id. The idempotent script already inserts the record as part
    /// of its guarded body; this reconciliation covers deployments where the
    /// record is missing while the schema is otherwise synchronized, so
    /// future EF migrations do not re-run against an already-deployed schema.
    /// </summary>
    static async Task ReconcileMigrationsHistoryAsync(
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        const string reconcileSql = $"""
            CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
                "MigrationId" character varying(150) NOT NULL,
                "ProductVersion" character varying(32) NOT NULL,
                CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
            );

            INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
            SELECT '{InitialMigrationId}', '{EfProductVersion}'
            WHERE NOT EXISTS (
                SELECT 1 FROM "__EFMigrationsHistory"
                WHERE "MigrationId" = '{InitialMigrationId}');
            """;

        await dbContext.Database.ExecuteSqlRawAsync(reconcileSql, cancellationToken);
    }
}
