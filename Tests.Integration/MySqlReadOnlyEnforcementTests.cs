using MySql.Data.MySqlClient;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Context;
using Tests.Integration.Fixtures;

namespace Tests.Integration;

/// <summary>
/// Verifies, against a real MySQL server, the two guarantees
/// <see cref="MySQLDapperContext"/> makes that a SQLite stand-in (used by the unit test suite
/// for query-shape testing) cannot: that "SET SESSION TRANSACTION READ ONLY" actually gets
/// enforced by the engine - rejecting a write outright rather than merely being unused - and
/// that the read queries the RemBillRep repositories issue really do run against MySQL's SQL
/// dialect (backtick-quoted identifiers included), not just against SQLite's tolerant parser.
/// </summary>
[Collection("Integration")]
public class MySqlReadOnlyEnforcementTests(ApiFactory apiFactory)
{
  readonly ApiFactory _apiFactory = apiFactory;

  /// <summary>
  /// Creates the table via a separate, ordinary (non-read-only) connection - the
  /// application's own connection factory marks its session read-only as soon as it opens,
  /// so it can never be used to set up its own fixture data. Pooling is deliberately
  /// disabled: ADO.NET connection pools are keyed by connection string, and MySql.Data does
  /// not reset "SET SESSION TRANSACTION READ ONLY" when a connection returns to the pool -
  /// without this, this "fresh" connection could be handed the exact physical connection the
  /// app's own read-only factory used and returned, inheriting its read-only session.
  /// </summary>
  async Task SeedClientCodTableAsync()
  {
    var connectionString = new MySqlConnectionStringBuilder(_apiFactory.MySqlConnectionString) { Pooling = false }.ConnectionString;
    await using var connection = new MySqlConnection(connectionString);
    await connection.OpenAsync();
    await using var command = connection.CreateCommand();
    command.CommandText = """
      CREATE TABLE IF NOT EXISTS `client_cod` (
        `id_client` INT PRIMARY KEY,
        `name` VARCHAR(100) NOT NULL,
        `nr_dogovor` VARCHAR(50) NOT NULL
      );
      """;
    await command.ExecuteNonQueryAsync();
  }

  [Fact]
  public async Task CreateConnectionAsync_OpensAgainstRealMySql_AndCanRunReadQueries()
  {
    await SeedClientCodTableAsync();

    var rowCount = await _apiFactory.WithScopedServiceAsync<IMySqlConnectionFactory, long>(async factory =>
    {
      var connection = await factory.CreateConnectionAsync(CancellationToken.None);
      using var command = connection.CreateCommand();
      command.CommandText = "SELECT COUNT(*) FROM `client_cod`";
      return Convert.ToInt64(command.ExecuteScalar());
    });

    rowCount.Should().Be(0);
  }

  [Fact]
  public async Task CreateConnectionAsync_SessionIsReadOnly_RejectsWrites()
  {
    await SeedClientCodTableAsync();

    Func<Task> act = () => _apiFactory.WithScopedServiceAsync<IMySqlConnectionFactory, int>(async factory =>
    {
      var connection = await factory.CreateConnectionAsync(CancellationToken.None);
      using var command = connection.CreateCommand();
      command.CommandText = "INSERT INTO `client_cod` (`id_client`, `name`, `nr_dogovor`) VALUES (1, 'Alpha', 'D-1')";
      return command.ExecuteNonQuery();
    });

    await act.Should().ThrowAsync<MySqlException>();
  }
}
