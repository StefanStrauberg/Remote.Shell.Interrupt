using System.Data;

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Context;

/// <summary>
/// Abstraction over connecting to the remote billing MySQL database, used exclusively
/// by the RemBillRep repositories to run read-only Dapper queries.
///
/// This database is owned and operated by a third party: its full schema is unknown to
/// this application, this application is not permitted to change it, and the only
/// contract that exists is "these specific columns, on these specific tables, can be
/// read". EF Core is deliberately not used here — EF Core wants to own and fully model a
/// schema (and, via migrations, evolve it), which does not fit a foreign, read-only,
/// partially-known data source. Dapper's "just run this SQL and map the columns I asked
/// for" model is the correct tool for this constraint, not an inconsistency with the
/// EF-Core-based PostgreSQL side of this project (which this application does own).
///
/// Exposing <see cref="IDbConnection"/> instead of the concrete <c>MySqlConnection</c>
/// lets repositories be unit tested against a fake connection (e.g. an in-memory
/// SQLite database standing in for the real MySQL schema) instead of requiring a live
/// connection to the remote database.
/// </summary>
internal interface IMySqlConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken);
}
