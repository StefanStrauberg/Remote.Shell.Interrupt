using Microsoft.EntityFrameworkCore;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.NetDevRep;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.Specification;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Helpers;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.NetDevRep;
using Remote.Shell.Interrupt.Storehouse.Specification.Specifications;

namespace Tests.Persistence;

internal static class TestDbContextFactory
{
    internal static ApplicationDbContext CreateContext()
        => new(new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString())
               .Options);
}

public class ILikeExpressionVisitorTests
{
    [Fact]
    public void Visit_StringContains_IsRewrittenToNpgsqlILike()
    {
        Expression<Func<Gate, bool>> expression = g => g.Name.Contains("gw");

        var visited = new ILikeExpressionVisitor().Visit(expression.Body);

        var call = visited.Should().BeAssignableTo<MethodCallExpression>().Subject;
        call.Method.Name.Should().Be("ILike");
    }

    [Fact]
    public void Visit_ContainsWholeWord_IsRewrittenToRegexIsMatch()
    {
        Expression<Func<Gate, bool>> expression = g => StringExtensions.ContainsWholeWord(g.Name, "gate");

        var visited = new ILikeExpressionVisitor().Visit(expression.Body);

        var call = visited.Should().BeAssignableTo<MethodCallExpression>().Subject;
        call.Method.Name.Should().Be("IsMatch");
    }

    [Fact]
    public void Visit_OtherMethodCalls_AreLeftUnchanged()
    {
        Expression<Func<Gate, bool>> expression = g => g.Name.StartsWith("gw");

        var visited = new ILikeExpressionVisitor().Visit(expression.Body);

        var call = visited.Should().BeAssignableTo<MethodCallExpression>().Subject;
        call.Method.Name.Should().Be("StartsWith");
    }
}

public class GenericRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context = TestDbContextFactory.CreateContext();

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task InsertAndRead_PersistEntity()
    {
        var gate = new Gate { Id = Guid.NewGuid(), Name = "gw-1", Community = "public" };
        var insert = new InsertRepository<Gate>(_context);
        var read = new ReadRepository<Gate>(_context);

        ((IInsertRepository<Gate>)insert).InsertOne(gate);
        await _context.SaveChangesAsync();
        var all = await ((IReadRepository<Gate>)read).GetAllAsync(CancellationToken.None);

        all.Should().ContainSingle(g => g.Name == "gw-1");
    }

    [Fact]
    public async Task DeleteOne_RemovesEntity()
    {
        var gate = new Gate { Id = Guid.NewGuid(), Name = "gw-del" };
        _context.Gates.Add(gate);
        await _context.SaveChangesAsync();

        var delete = new DeleteRepository<Gate>(_context);
        ((IDeleteRepository<Gate>)delete).DeleteOne(gate);
        await _context.SaveChangesAsync();

        _context.Gates.Should().BeEmpty();
    }

    [Fact]
    public async Task ReplaceOne_UpdatesEntity()
    {
        var gate = new Gate { Id = Guid.NewGuid(), Name = "before", Community = "c" };
        _context.Gates.Add(gate);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        gate.Name = "after";
        var replace = new ReplaceRepository<Gate>(_context);
        ((IReplaceRepository<Gate>)replace).ReplaceOne(gate);
        await _context.SaveChangesAsync();

        var stored = await _context.Gates.AsNoTracking().SingleAsync(g => g.Id == gate.Id);
        stored.Name.Should().Be("after");
    }

    [Fact]
    public async Task BulkInsert_InsertsAllEntities()
    {
        var gates = new List<Gate>
        {
            new() { Id = Guid.NewGuid(), Name = "a" },
            new() { Id = Guid.NewGuid(), Name = "b" },
            new() { Id = Guid.NewGuid(), Name = "c" }
        };
        var bulkInsert = new BulkInsertRepository<Gate>(_context);
        ((IBulkInsertRepository<Gate>)bulkInsert).InsertMany(gates);
        await _context.SaveChangesAsync();

        _context.Gates.Should().HaveCount(3);
    }

    [Fact]
    public async Task BulkDeleteAndBulkReplace_ManageManyEntities()
    {
        var gates = new List<Gate>
        {
            new() { Id = Guid.NewGuid(), Name = "a" },
            new() { Id = Guid.NewGuid(), Name = "b" },
            new() { Id = Guid.NewGuid(), Name = "c" }
        };
        _context.Gates.AddRange(gates);
        await _context.SaveChangesAsync();

        var bulkReplace = new BulkReplaceRepository<Gate>(_context);
        gates[0].Name = "a2";
        ((IBulkReplaceRepository<Gate>)bulkReplace).ReplaceMany([gates[0]]);
        var bulkDelete = new BulkDeleteRepository<Gate>(_context);
        ((IBulkDeleteRepository<Gate>)bulkDelete).DeleteMany([gates[1]]);
        await _context.SaveChangesAsync();

        var names = await _context.Gates.AsNoTracking().Select(g => g.Name).ToListAsync();
        names.Should().BeEquivalentTo(["a2", "c"]);
    }

    [Fact]
    public async Task CountRepository_WithCriteria_CountsOnlyMatching()
    {
        _context.Gates.AddRange(
            new Gate { Id = Guid.NewGuid(), Name = "target" },
            new Gate { Id = Guid.NewGuid(), Name = "target" },
            new Gate { Id = Guid.NewGuid(), Name = "other" });
        await _context.SaveChangesAsync();

        var spec = new GenericSpecification<Gate>();
        spec.AddFilter(g => g.Name == "target");
        var repo = new CountRepository<Gate>(_context);

        var count = await ((ICountRepository<Gate>)repo).GetCountAsync(spec, CancellationToken.None);

        count.Should().Be(2);
    }

    [Fact]
    public async Task ExistenceQueryRepository_ReflectsDataPresence()
    {
        var repo = new ExistenceQueryRepository<Gate>(_context);
        var spec = new GenericSpecification<Gate>();
        spec.AddFilter(g => g.Name == "missing");

        var before = await ((IExistenceQueryRepository<Gate>)repo).AnyByQueryAsync(spec, CancellationToken.None);

        _context.Gates.Add(new Gate { Id = Guid.NewGuid(), Name = "missing" });
        await _context.SaveChangesAsync();
        var after = await ((IExistenceQueryRepository<Gate>)repo).AnyByQueryAsync(spec, CancellationToken.None);

        before.Should().BeFalse();
        after.Should().BeTrue();
    }

    [Fact]
    public async Task ManyQueryRepository_AppliesOrderSkipAndTake()
    {
        for (var i = 1; i <= 5; i++)
            _context.Gates.Add(new Gate { Id = Guid.NewGuid(), Name = $"gw-{i:D2}" });
        await _context.SaveChangesAsync();

        var spec = new GenericSpecification<Gate>();
        spec.AddOrderBy(g => g.Name);
        spec.ConfigurePagination(new PaginationContext(2, 2));
        var repo = new ManyQueryRepository<Gate>(_context);

        var page = await ((IManyQueryRepository<Gate>)repo).GetManyShortAsync(spec, CancellationToken.None);

        page.Select(g => g.Name).Should().ContainInOrder("gw-03", "gw-04");
    }

    [Fact]
    public async Task OneQueryRepository_EmptyResult_ThrowsInvalidOperationException()
    {
        var spec = new GenericSpecification<Gate>();
        spec.AddFilter(g => g.Name == "nothing");
        var repo = new OneQueryRepository<Gate>(_context);

        var act = async () => await ((IOneQueryRepository<Gate>)repo).GetOneShortAsync(spec, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}

public class NetworkDeviceRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context = TestDbContextFactory.CreateContext();

    public void Dispose() => _context.Dispose();

    NetworkDeviceRepository CreateRepository()
        => new(_context,
               Substitute.For<IManyQueryRepository<NetworkDevice>>(),
               Substitute.For<IExistenceQueryRepository<NetworkDevice>>(),
               Substitute.For<ICountRepository<NetworkDevice>>(),
               Substitute.For<IInsertRepository<NetworkDevice>>(),
               Substitute.For<IReadRepository<NetworkDevice>>());

    static (NetworkDevice Device, Port Parent, Port Child) BuildDeviceGraph()
    {
        var device = new NetworkDevice { Id = Guid.NewGuid(), Host = 1, NetworkDeviceName = "gw" };
        var parent = new Port { Id = Guid.NewGuid(), NetworkDeviceId = device.Id, InterfaceName = "ae0" };
        var child = new Port { Id = Guid.NewGuid(), NetworkDeviceId = device.Id, InterfaceName = "xe-0/0/0.0", ParentId = parent.Id };
        parent.AggregatedPorts.Add(child);
        return (device, parent, child);
    }

    [Fact]
    public async Task DeleteOneWithChildren_RemovesDevicePortsAndAllChildEntities()
    {
        var (device, parent, child) = BuildDeviceGraph();
        var arp = new ARPEntity { Id = Guid.NewGuid(), PortId = child.Id, MAC = "m", IPAddress = "1.2.3.4" };
        var mac = new MACEntity { Id = Guid.NewGuid(), PortId = child.Id, MACAddress = "m" };
        var terminated = new TerminatedNetworkEntity { Id = Guid.NewGuid(), PortId = parent.Id };
        var vlan = new VLAN { Id = Guid.NewGuid(), VLANTag = 100, VLANName = "CLIENTS" };
        parent.VLANs.Add(vlan);
        vlan.Ports.Add(parent);
        child.ARPTableOfInterface.Add(arp);
        _context.AddRange(device, parent, child, arp, mac, terminated, vlan);
        await _context.SaveChangesAsync();

        var repo = CreateRepository();
        ((INetworkDeviceRepository)repo).DeleteOneWithChildren(device);
        await _context.SaveChangesAsync();

        _context.NetworkDevices.Should().BeEmpty();
        _context.Ports.Should().BeEmpty();
        _context.ARPEntities.Should().BeEmpty();
        _context.MACEntities.Should().BeEmpty();
        _context.TerminatedNetworkEntities.Should().BeEmpty();
        _context.VLANs.Should().BeEmpty();
    }

    [Fact]
    public async Task GetOneWithChildrenAsync_AppliesSpecificationIncludes()
    {
        var (device, parent, child) = BuildDeviceGraph();
        var arp = new ARPEntity { Id = Guid.NewGuid(), PortId = child.Id, MAC = "m", IPAddress = "1.2.3.4" };
        child.ARPTableOfInterface.Add(arp);
        _context.AddRange(device, parent, child, arp);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var spec = new NetworkDeviceSpecification();
        spec.AddInclude(d => d.PortsOfNetworkDevice);
        spec.AddThenInclude<Port, IEnumerable<ARPEntity>>(p => p.ARPTableOfInterface);
        var repo = CreateRepository();

        var fetched = await ((IOneQueryWithRelationsRepository<NetworkDevice>)repo)
            .GetOneWithChildrenAsync(spec, CancellationToken.None);

        fetched.Id.Should().Be(device.Id);
        fetched.PortsOfNetworkDevice.Should().HaveCount(2);
        fetched.PortsOfNetworkDevice.Single(p => p.Id == child.Id).ARPTableOfInterface.Should().ContainSingle();
    }

    [Fact]
    public async Task GetManyWithChildrenAsync_AppliesFilterAndIncludes()
    {
        var (device, parent, child) = BuildDeviceGraph();
        var other = new NetworkDevice { Id = Guid.NewGuid(), Host = 2, NetworkDeviceName = "other" };
        _context.AddRange(device, parent, child, other);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var spec = new NetworkDeviceSpecification();
        spec.AddInclude(d => d.PortsOfNetworkDevice);
        spec.AddFilter(d => d.NetworkDeviceName == "gw");
        var repo = CreateRepository();

        var result = await ((IManyQueryWithRelationsRepository<NetworkDevice>)repo)
            .GetManyWithChildrenAsync(spec, CancellationToken.None);

        result.Should().ContainSingle(d => d.Id == device.Id);
        result.First().PortsOfNetworkDevice.Should().HaveCount(2);
    }
}

public class PortRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context = TestDbContextFactory.CreateContext();

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task GetAllAggregatedPortsByListAsync_ReturnsChildrenOfGivenParents()
    {
        var parent1 = new Port { Id = Guid.NewGuid(), InterfaceName = "ae0", NetworkDeviceId = Guid.NewGuid() };
        var parent2 = new Port { Id = Guid.NewGuid(), InterfaceName = "ae1", NetworkDeviceId = parent1.NetworkDeviceId };
        var child1 = new Port { Id = Guid.NewGuid(), InterfaceName = "xe-0/0/0", ParentId = parent1.Id, NetworkDeviceId = parent1.NetworkDeviceId };
        var child2 = new Port { Id = Guid.NewGuid(), InterfaceName = "xe-0/0/1", ParentId = parent1.Id, NetworkDeviceId = parent1.NetworkDeviceId };
        var child3 = new Port { Id = Guid.NewGuid(), InterfaceName = "xe-0/1/0", ParentId = parent2.Id, NetworkDeviceId = parent1.NetworkDeviceId };
        // Only ports with a ParentId are seeded: the InMemory provider evaluates the
        // ParentId.Value access client-side, so rows with a null ParentId are excluded
        // (the production PostgreSQL provider applies SQL null semantics instead).
        _context.AddRange(child1, child2, child3);
        await _context.SaveChangesAsync();
        var repo = new PortRepository(_context,
                                      Substitute.For<IExistenceQueryRepository<Port>>(),
                                      Substitute.For<IOneQueryRepository<Port>>(),
                                      Substitute.For<IBulkInsertRepository<Port>>(),
                                      Substitute.For<IBulkDeleteRepository<Port>>(),
                                      Substitute.For<IBulkReplaceRepository<Port>>());

        var result = await ((IPortRepository)repo)
            .GetAllAggregatedPortsByListAsync([parent1.Id], CancellationToken.None);

        var children = result.ToList();
        children.Should().HaveCount(2);
        children.Should().OnlyContain(p => p.ParentId == parent1.Id);
    }
}
