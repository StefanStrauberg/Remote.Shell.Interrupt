using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.IGateRep;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.LocBillRep;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.UnOfWrkRep;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Context;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.IGateRep;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.LocBillRep;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.NetDevRep;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.RemBillRep;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.UnOfWrkRep;

namespace Tests.Persistence;

public class GateUnitOfWorkTests : IDisposable
{
    private readonly ApplicationDbContext _context = TestDbContextFactory.CreateContext();

    public void Dispose() => _context.Dispose();

    GateUnitOfWork CreateUnitOfWork()
        => new(_context,
               Substitute.For<IExistenceQueryRepository<Gate>>(),
               Substitute.For<ICountRepository<Gate>>(),
               Substitute.For<IManyQueryRepository<Gate>>(),
               Substitute.For<IOneQueryRepository<Gate>>(),
               Substitute.For<IInsertRepository<Gate>>(),
               Substitute.For<IDeleteRepository<Gate>>(),
               Substitute.For<IReplaceRepository<Gate>>());

    [Fact]
    public void Gates_ReturnsGateRepository()
    {
        var uow = CreateUnitOfWork();

        var gates = ((IGateUnitOfWork)uow).Gates;

        gates.Should().NotBeNull();
        gates.Should().BeOfType<GateRepository>();
    }

    [Fact]
    public async Task Complete_PersistsPendingChanges()
    {
        _context.Gates.Add(new Gate { Id = Guid.NewGuid(), Name = "gw" });
        var uow = CreateUnitOfWork();

        ((IUnitOfWork)uow).Complete();

        var stored = await _context.Gates.AsNoTracking().ToListAsync();
        stored.Should().ContainSingle(g => g.Name == "gw");
    }

    [Fact]
    public async Task CompleteAsync_PersistsPendingChanges()
    {
        _context.Gates.Add(new Gate { Id = Guid.NewGuid(), Name = "gw-async" });
        var uow = CreateUnitOfWork();

        await ((IUnitOfWork)uow).CompleteAsync(CancellationToken.None);

        var stored = await _context.Gates.AsNoTracking().ToListAsync();
        stored.Should().ContainSingle(g => g.Name == "gw-async");
    }

    [Fact]
    public void StartTransaction_ThrowsOnInMemoryProvider()
    {
        // The EF Core InMemory provider does not support relational transactions;
        // BeginTransaction() throws InvalidOperationException for this provider.
        var uow = CreateUnitOfWork();

        var act = () => ((IUnitOfWork)uow).StartTransaction();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var uow = CreateUnitOfWork();

        var act = () => ((IDisposable)uow).Dispose();

        act.Should().NotThrow();
    }
}

public class LocBillUnitOfWorkTests : IDisposable
{
    private readonly ApplicationDbContext _context = TestDbContextFactory.CreateContext();

    public void Dispose() => _context.Dispose();

    LocBillUnitOfWork CreateUnitOfWork()
        => new(_context,
               Substitute.For<ICountRepository<Client>>(),
               Substitute.For<IExistenceQueryRepository<Client>>(),
               Substitute.For<IManyQueryRepository<Client>>(),
               Substitute.For<IReadRepository<Client>>(),
               Substitute.For<IBulkDeleteRepository<Client>>(),
               Substitute.For<IBulkInsertRepository<Client>>(),
               Substitute.For<IBulkDeleteRepository<COD>>(),
               Substitute.For<IReadRepository<COD>>(),
               Substitute.For<IBulkInsertRepository<COD>>(),
               Substitute.For<IManyQueryRepository<TfPlan>>(),
               Substitute.For<ICountRepository<TfPlan>>(),
               Substitute.For<IReadRepository<TfPlan>>(),
               Substitute.For<IBulkDeleteRepository<TfPlan>>(),
               Substitute.For<IBulkInsertRepository<TfPlan>>(),
               Substitute.For<IManyQueryRepository<SPRVlan>>(),
               Substitute.For<ICountRepository<SPRVlan>>(),
               Substitute.For<IReadRepository<SPRVlan>>(),
               Substitute.For<IBulkDeleteRepository<SPRVlan>>(),
               Substitute.For<IBulkInsertRepository<SPRVlan>>());

    [Fact]
    public void Clients_ReturnsClientsRepository()
    {
        var uow = CreateUnitOfWork();

        var clients = ((ILocBillUnitOfWork)uow).Clients;

        clients.Should().NotBeNull();
        clients.Should().BeOfType<ClientsRepository>();
    }

    [Fact]
    public void CODs_ReturnsCODRepository()
    {
        var uow = CreateUnitOfWork();

        var cods = ((ILocBillUnitOfWork)uow).CODs;

        cods.Should().NotBeNull();
        cods.Should().BeOfType<CODRepository>();
    }

    [Fact]
    public void TfPlans_ReturnsTfPlanRepository()
    {
        var uow = CreateUnitOfWork();

        var tfPlans = ((ILocBillUnitOfWork)uow).TfPlans;

        tfPlans.Should().NotBeNull();
        tfPlans.Should().BeOfType<TfPlanRepository>();
    }

    [Fact]
    public void SPRVlans_ReturnsSPRVlansRepository()
    {
        var uow = CreateUnitOfWork();

        var sprVlans = ((ILocBillUnitOfWork)uow).SPRVlans;

        sprVlans.Should().NotBeNull();
        sprVlans.Should().BeOfType<SPRVlansRepository>();
    }

    [Fact]
    public async Task Complete_PersistsPendingChanges()
    {
        _context.CODs.Add(new COD { Id = Guid.NewGuid(), NameCOD = "cod" });
        var uow = CreateUnitOfWork();

        ((IUnitOfWork)uow).Complete();

        var stored = await _context.CODs.AsNoTracking().ToListAsync();
        stored.Should().ContainSingle(c => c.NameCOD == "cod");
    }

    [Fact]
    public async Task CompleteAsync_PersistsPendingChanges()
    {
        _context.CODs.Add(new COD { Id = Guid.NewGuid(), NameCOD = "cod-async" });
        var uow = CreateUnitOfWork();

        await ((IUnitOfWork)uow).CompleteAsync(CancellationToken.None);

        var stored = await _context.CODs.AsNoTracking().ToListAsync();
        stored.Should().ContainSingle(c => c.NameCOD == "cod-async");
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var uow = CreateUnitOfWork();

        var act = () => ((IDisposable)uow).Dispose();

        act.Should().NotThrow();
    }
}

public class NetDevUnitOfWorkTests : IDisposable
{
    private readonly ApplicationDbContext _context = TestDbContextFactory.CreateContext();

    public void Dispose() => _context.Dispose();

    NetDevUnitOfWork CreateUnitOfWork()
        => new(_context,
               Substitute.For<IManyQueryRepository<NetworkDevice>>(),
               Substitute.For<IExistenceQueryRepository<NetworkDevice>>(),
               Substitute.For<ICountRepository<NetworkDevice>>(),
               Substitute.For<IInsertRepository<NetworkDevice>>(),
               Substitute.For<IReadRepository<NetworkDevice>>(),
               Substitute.For<IBulkInsertRepository<VLAN>>(),
               Substitute.For<IExistenceQueryRepository<Port>>(),
               Substitute.For<IOneQueryRepository<Port>>(),
               Substitute.For<IBulkInsertRepository<Port>>(),
               Substitute.For<IBulkDeleteRepository<Port>>(),
               Substitute.For<IBulkReplaceRepository<Port>>(),
               Substitute.For<IBulkInsertRepository<ARPEntity>>(),
               Substitute.For<IBulkInsertRepository<MACEntity>>(),
               Substitute.For<IBulkInsertRepository<TerminatedNetworkEntity>>());

    [Fact]
    public void NetworkDevices_ReturnsNetworkDeviceRepository()
    {
        var uow = CreateUnitOfWork();

        uow.NetworkDevices.Should().NotBeNull();
        uow.NetworkDevices.Should().BeOfType<NetworkDeviceRepository>();
    }

    [Fact]
    public void VLANs_ReturnsVLANRepository()
    {
        var uow = CreateUnitOfWork();

        uow.VLANs.Should().NotBeNull();
        uow.VLANs.Should().BeOfType<VLANRepository>();
    }

    [Fact]
    public void Ports_ReturnsPortRepository()
    {
        var uow = CreateUnitOfWork();

        uow.Ports.Should().NotBeNull();
        uow.Ports.Should().BeOfType<PortRepository>();
    }

    [Fact]
    public void ARPEntities_ReturnsARPEntityRepository()
    {
        var uow = CreateUnitOfWork();

        uow.ARPEntities.Should().NotBeNull();
        uow.ARPEntities.Should().BeOfType<ARPEntityRepository>();
    }

    [Fact]
    public void MACEntities_ReturnsMACEntityRepository()
    {
        var uow = CreateUnitOfWork();

        uow.MACEntities.Should().NotBeNull();
        uow.MACEntities.Should().BeOfType<MACEntityRepository>();
    }

    [Fact]
    public void TerminatedNetworkEntities_ReturnsTerminatedNetworkEntityRepository()
    {
        var uow = CreateUnitOfWork();

        uow.TerminatedNetworkEntities.Should().NotBeNull();
        uow.TerminatedNetworkEntities.Should().BeOfType<TerminatedNetworkEntityRepository>();
    }

    [Fact]
    public async Task Complete_PersistsPendingChanges()
    {
        _context.VLANs.Add(new VLAN { Id = Guid.NewGuid(), VLANTag = 5, VLANName = "v5" });
        var uow = CreateUnitOfWork();

        ((IUnitOfWork)uow).Complete();

        var stored = await _context.VLANs.AsNoTracking().ToListAsync();
        stored.Should().ContainSingle(v => v.VLANName == "v5");
    }

    [Fact]
    public async Task CompleteAsync_PersistsPendingChanges()
    {
        _context.VLANs.Add(new VLAN { Id = Guid.NewGuid(), VLANTag = 6, VLANName = "v6-async" });
        var uow = CreateUnitOfWork();

        await ((IUnitOfWork)uow).CompleteAsync(CancellationToken.None);

        var stored = await _context.VLANs.AsNoTracking().ToListAsync();
        stored.Should().ContainSingle(v => v.VLANName == "v6-async");
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var uow = CreateUnitOfWork();

        var act = () => ((IDisposable)uow).Dispose();

        act.Should().NotThrow();
    }
}

public class RemBillUnitOfWorkTests
{
    // RemoteClientsRepository/RemoteCODRepository/etc. are internal to the Dapper.Persistence
    // assembly. NSubstitute proxies are emitted into a separate dynamic assembly that has no
    // InternalsVisibleTo grant from Dapper.Persistence, so Substitute.For<IAppLogger<TInternal>>()
    // fails with "type is not accessible". A trivial no-op implementation avoids dynamic proxying.
    class NullAppLogger<T> : IAppLogger<T>
    {
        public void LogInformation(string message, params object[] args) { }
        public void LogWarning(string message, params object[] args) { }
        public void LogError(string message, params object[] args) { }
        public void LogError(Exception exception, string message, params object[] args) { }
    }

    static MySQLDapperContext CreateDapperContext()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection2"] = "Server=localhost;Database=test;Uid=test;Pwd=test;"
            })
            .Build();

        return new MySQLDapperContext(configuration);
    }

    RemBillUnitOfWork CreateUnitOfWork()
        => new(CreateDapperContext(),
               new NullAppLogger<RemoteClientsRepository>(),
               new NullAppLogger<RemoteCODRepository>(),
               new NullAppLogger<RemoteTfPlanRepository>(),
               new NullAppLogger<RemoteSPRVlansRepository>());

    [Fact]
    public void RemoteClients_ReturnsRemoteClientsRepository()
    {
        var uow = CreateUnitOfWork();

        uow.RemoteClients.Should().NotBeNull();
        uow.RemoteClients.Should().BeOfType<RemoteClientsRepository>();
    }

    [Fact]
    public void RemoteCODs_ReturnsRemoteCODRepository()
    {
        var uow = CreateUnitOfWork();

        uow.RemoteCODs.Should().NotBeNull();
        uow.RemoteCODs.Should().BeOfType<RemoteCODRepository>();
    }

    [Fact]
    public void RemoteTfPlans_ReturnsRemoteTfPlanRepository()
    {
        var uow = CreateUnitOfWork();

        uow.RemoteTfPlans.Should().NotBeNull();
        uow.RemoteTfPlans.Should().BeOfType<RemoteTfPlanRepository>();
    }

    [Fact]
    public void RemoteSPRVlans_ReturnsRemoteSPRVlansRepository()
    {
        var uow = CreateUnitOfWork();

        uow.RemoteSPRVlans.Should().NotBeNull();
        uow.RemoteSPRVlans.Should().BeOfType<RemoteSPRVlansRepository>();
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        // MySQLDapperContext is intentionally not disposed by RemBillUnitOfWork (DI-scoped
        // lifetime), so Dispose() here is a pure no-op that flips the internal flag.
        var uow = CreateUnitOfWork();

        var act = () => ((IDisposable)uow).Dispose();

        act.Should().NotThrow();
    }
}
