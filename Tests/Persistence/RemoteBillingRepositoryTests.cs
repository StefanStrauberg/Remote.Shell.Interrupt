using System.Data;
using Microsoft.Data.Sqlite;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Context;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.RemBillRep;

namespace Tests.Persistence;

/// <summary>
/// Exercises the RemBillRep repositories' actual SQL (table/column names, aliasing)
/// against an in-memory SQLite database standing in for the real, externally-owned
/// MySQL schema this application only has narrow read access to. SQLite accepts the
/// same backtick-quoted identifiers and "AS &quot;Alias&quot;" syntax these queries use for
/// MySQL, so the same query text runs unmodified — this catches a mistyped column or
/// table name the same way running against the real database would, without ever
/// connecting to it.
/// </summary>
public class RemoteBillingRepositoryTests : IDisposable
{
    readonly SqliteConnection _connection = new("Data Source=:memory:");

    public RemoteBillingRepositoryTests() => _connection.Open();

    public void Dispose() => _connection.Dispose();

    IMySqlConnectionFactory CreateFactory() => new FakeMySqlConnectionFactory(_connection);

    /// <summary>
    /// Hand-rolled instead of an NSubstitute mock: <see cref="IMySqlConnectionFactory"/>
    /// is internal, and NSubstitute's Castle proxy generator needs
    /// [InternalsVisibleTo("DynamicProxyGenAssembly2")] to implement an internal
    /// interface, which this assembly does not (and should not) grant.
    /// </summary>
    sealed class FakeMySqlConnectionFactory(IDbConnection connection) : IMySqlConnectionFactory
    {
        public Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken)
            => Task.FromResult(connection);
    }

    /// <summary>
    /// Hand-rolled for the same reason as <see cref="FakeMySqlConnectionFactory"/>: the
    /// generic argument (an internal RemBillRep repository type) makes
    /// Substitute.For&lt;IAppLogger&lt;T&gt;&gt;() fail the same way.
    /// </summary>
    sealed class NullAppLogger<T> : IAppLogger<T>
    {
        public void LogInformation(string message, params object[] args) { }
        public void LogWarning(string message, params object[] args) { }
        public void LogError(string message, params object[] args) { }
        public void LogError(Exception exception, string message, params object[] args) { }
    }

    void Execute(string sql)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    [Fact]
    public async Task RemoteClientsRepository_GetAllAsync_MapsAllColumnsFromClientCodTable()
    {
        Execute("""
            CREATE TABLE client_cod (
                id_client INTEGER, dat1 TEXT, dat2 TEXT, prim1 TEXT, prim2 TEXT, nik TEXT,
                name TEXT, nr_dogovor TEXT, contact_C TEXT, telefon_C TEXT, contact_T TEXT,
                telefon_T TEXT, c_email TEXT, `_working` INTEGER, t_email TEXT,
                id_cod INTEGER, id_tplan INTEGER, history TEXT, ad INTEGER
            );
            INSERT INTO client_cod VALUES (
                5, '2024-01-02T03:04:05', NULL, 'p1', 'p2', 'nik-1',
                'Alpha', 'D-5', 'cc', 'tc', 'ct',
                'tt', 'c@test.com', 1, 't@test.com',
                11, 22, 'log', 0
            );
            """);
        var repo = new RemoteClientsRepository(CreateFactory(), new NullAppLogger<RemoteClientsRepository>());

        var result = await ((IRemoteGenericRepository<RemoteClient>)repo).GetAllAsync(CancellationToken.None);

        var client = result.Should().ContainSingle().Subject;
        client.IdClient.Should().Be(5);
        client.Dat1.Should().Be(new DateTime(2024, 1, 2, 3, 4, 5));
        client.Dat2.Should().BeNull();
        client.Name.Should().Be("Alpha");
        client.NrDogovor.Should().Be("D-5");
        client.Working.Should().BeTrue();
        client.AntiDDOS.Should().BeFalse();
        client.Id_COD.Should().Be(11);
        client.Id_TfPlan.Should().Be(22);
        client.History.Should().Be("log");
    }

    [Fact]
    public async Task RemoteCODRepository_GetAllAsync_MapsAllColumnsFromCodsTable()
    {
        Execute("""
            CREATE TABLE `_cods` (
                ID_cod INTEGER, name_cod TEXT, telephone TEXT, e_mail TEXT,
                e_mail2 TEXT, contact TEXT, description TEXT, region TEXT
            );
            INSERT INTO `_cods` VALUES (1, 'DC-1', '123', 'a@test.com', 'b@test.com', 'contact', 'desc', 'region');
            """);
        var repo = new RemoteCODRepository(CreateFactory(), new NullAppLogger<RemoteCODRepository>());

        var result = await ((IRemoteGenericRepository<RemoteCOD>)repo).GetAllAsync(CancellationToken.None);

        var cod = result.Should().ContainSingle().Subject;
        cod.IdCOD.Should().Be(1);
        cod.NameCOD.Should().Be("DC-1");
        cod.Email1.Should().Be("a@test.com");
        cod.Email2.Should().Be("b@test.com");
        cod.Region.Should().Be("region");
    }

    [Fact]
    public async Task RemoteSPRVlansRepository_GetAllAsync_MapsAllColumnsFromSprVlanTable()
    {
        Execute("""
            CREATE TABLE `_spr_vlan` (id_vlan INTEGER, id_client INTEGER, use_client INTEGER, use_cod INTEGER);
            INSERT INTO `_spr_vlan` VALUES (100, 5, 1, 0);
            """);
        var repo = new RemoteSPRVlansRepository(CreateFactory(), new NullAppLogger<RemoteSPRVlansRepository>());

        var result = await ((IRemoteGenericRepository<RemoteSPRVlan>)repo).GetAllAsync(CancellationToken.None);

        var vlan = result.Should().ContainSingle().Subject;
        vlan.IdVlan.Should().Be(100);
        vlan.IdClient.Should().Be(5);
        vlan.UseClient.Should().BeTrue();
        vlan.UseCOD.Should().BeFalse();
    }

    [Fact]
    public async Task RemoteTfPlanRepository_GetAllAsync_MapsAllColumnsFromTfPlanTable()
    {
        Execute("""
            CREATE TABLE `_tf_plan` (id_tplan INTEGER, name_tplan TEXT, descr_tplan TEXT);
            INSERT INTO `_tf_plan` VALUES (22, 'Basic', 'A basic plan');
            """);
        var repo = new RemoteTfPlanRepository(CreateFactory(), new NullAppLogger<RemoteTfPlanRepository>());

        var result = await ((IRemoteGenericRepository<RemoteTfPlan>)repo).GetAllAsync(CancellationToken.None);

        var plan = result.Should().ContainSingle().Subject;
        plan.IdTfPlan.Should().Be(22);
        plan.NameTfPlan.Should().Be("Basic");
        plan.DescTfPlan.Should().Be("A basic plan");
    }

    [Fact]
    public async Task RemoteClientsRepository_GetAllAsync_NoRows_ReturnsEmpty()
    {
        Execute("""
            CREATE TABLE client_cod (
                id_client INTEGER, dat1 TEXT, dat2 TEXT, prim1 TEXT, prim2 TEXT, nik TEXT,
                name TEXT, nr_dogovor TEXT, contact_C TEXT, telefon_C TEXT, contact_T TEXT,
                telefon_T TEXT, c_email TEXT, `_working` INTEGER, t_email TEXT,
                id_cod INTEGER, id_tplan INTEGER, history TEXT, ad INTEGER
            );
            """);
        var repo = new RemoteClientsRepository(CreateFactory(), new NullAppLogger<RemoteClientsRepository>());

        var result = await ((IRemoteGenericRepository<RemoteClient>)repo).GetAllAsync(CancellationToken.None);

        result.Should().BeEmpty();
    }
}
