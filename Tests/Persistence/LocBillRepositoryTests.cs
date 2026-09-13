using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.LocBillRep;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.LocBillRep;
using Remote.Shell.Interrupt.Storehouse.Specification.Specifications;

namespace Tests.Persistence;

public class ClientsRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context = TestDbContextFactory.CreateContext();
    private readonly ICountRepository<Client> _countRepository = Substitute.For<ICountRepository<Client>>();
    private readonly IExistenceQueryRepository<Client> _existenceQueryRepository = Substitute.For<IExistenceQueryRepository<Client>>();
    private readonly IManyQueryRepository<Client> _manyQueryRepository = Substitute.For<IManyQueryRepository<Client>>();
    private readonly IReadRepository<Client> _readRepository = Substitute.For<IReadRepository<Client>>();
    private readonly IBulkDeleteRepository<Client> _bulkDeleteRepository = Substitute.For<IBulkDeleteRepository<Client>>();
    private readonly IBulkInsertRepository<Client> _bulkInsertRepository = Substitute.For<IBulkInsertRepository<Client>>();

    public void Dispose() => _context.Dispose();

    ClientsRepository CreateRepository()
        => new(_context,
               _countRepository,
               _existenceQueryRepository,
               _manyQueryRepository,
               _readRepository,
               _bulkDeleteRepository,
               _bulkInsertRepository);

    static (COD Cod, Client Client) BuildClientWithCod(string name)
    {
        var cod = new COD { Id = Guid.NewGuid(), IdCOD = new Random().Next(1, 1_000_000), NameCOD = "cod-" + name };
        var client = new Client
        {
            Id = Guid.NewGuid(),
            IdClient = new Random().Next(1, 1_000_000),
            Name = name,
            NrDogovor = "contract-" + name,
            Id_COD = cod.IdCOD,
            COD = cod
        };
        return (cod, client);
    }

    [Fact]
    public async Task GetManyShortAsync_DelegatesToInjectedRepository()
    {
        var spec = new GenericSpecification<Client>();
        var repo = CreateRepository();

        await ((IManyQueryRepository<Client>)repo).GetManyShortAsync(spec, CancellationToken.None);

        await _manyQueryRepository.Received(1).GetManyShortAsync(spec, CancellationToken.None);
    }

    [Fact]
    public async Task AnyByQueryAsync_DelegatesToInjectedRepository()
    {
        var spec = new GenericSpecification<Client>();
        var repo = CreateRepository();

        await ((IExistenceQueryRepository<Client>)repo).AnyByQueryAsync(spec, CancellationToken.None);

        await _existenceQueryRepository.Received(1).AnyByQueryAsync(spec, CancellationToken.None);
    }

    [Fact]
    public async Task GetAllAsync_DelegatesToInjectedRepository()
    {
        var repo = CreateRepository();

        await ((IReadRepository<Client>)repo).GetAllAsync(CancellationToken.None);

        await _readRepository.Received(1).GetAllAsync(CancellationToken.None);
    }

    [Fact]
    public void DeleteMany_DelegatesToInjectedRepository()
    {
        var clients = new List<Client> { new() { Id = Guid.NewGuid(), Name = "a" } };
        var repo = CreateRepository();

        ((IBulkDeleteRepository<Client>)repo).DeleteMany(clients);

        _bulkDeleteRepository.Received(1).DeleteMany(clients);
    }

    [Fact]
    public void InsertMany_DelegatesToInjectedRepository()
    {
        var clients = new List<Client> { new() { Id = Guid.NewGuid(), Name = "a" } };
        var repo = CreateRepository();

        ((IBulkInsertRepository<Client>)repo).InsertMany(clients);

        _bulkInsertRepository.Received(1).InsertMany(clients);
    }

    [Fact]
    public async Task GetCountAsync_DelegatesToInjectedRepository()
    {
        var spec = new GenericSpecification<Client>();
        var repo = CreateRepository();

        await ((ICountRepository<Client>)repo).GetCountAsync(spec, CancellationToken.None);

        await _countRepository.Received(1).GetCountAsync(spec, CancellationToken.None);
    }

    [Fact]
    public async Task GetOneWithChildrenAsync_ReturnsMatchingClient()
    {
        var (cod1, client1) = BuildClientWithCod("first");
        var (cod2, client2) = BuildClientWithCod("second");
        _context.AddRange(cod1, client1, cod2, client2);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var spec = new GenericSpecification<Client>();
        spec.AddFilter(c => c.Name == "second");
        var repo = CreateRepository();

        var fetched = await ((IOneQueryWithRelationsRepository<Client>)repo)
            .GetOneWithChildrenAsync(spec, CancellationToken.None);

        fetched.Id.Should().Be(client2.Id);
    }

    [Fact]
    public async Task GetManyWithChildrenAsync_AppliesFilterAndPagination()
    {
        var (cod1, client1) = BuildClientWithCod("alpha");
        var (cod2, client2) = BuildClientWithCod("alpha");
        var (cod3, client3) = BuildClientWithCod("beta");
        _context.AddRange(cod1, client1, cod2, client2, cod3, client3);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var spec = new GenericSpecification<Client>();
        spec.AddFilter(c => c.Name == "alpha");
        var repo = CreateRepository();

        var result = await ((IManyQueryWithRelationsRepository<Client>)repo)
            .GetManyWithChildrenAsync(spec, CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(c => c.Name == "alpha");
    }
}

public class CODRepositoryTests
{
    private readonly IBulkDeleteRepository<COD> _bulkDeleteRepository = Substitute.For<IBulkDeleteRepository<COD>>();
    private readonly IReadRepository<COD> _readRepository = Substitute.For<IReadRepository<COD>>();
    private readonly IBulkInsertRepository<COD> _bulkInsertRepository = Substitute.For<IBulkInsertRepository<COD>>();

    CODRepository CreateRepository()
        => new(_bulkDeleteRepository, _readRepository, _bulkInsertRepository);

    [Fact]
    public void DeleteMany_DelegatesToInjectedRepository()
    {
        var cods = new List<COD> { new() { Id = Guid.NewGuid(), NameCOD = "a" } };
        var repo = CreateRepository();

        ((IBulkDeleteRepository<COD>)repo).DeleteMany(cods);

        _bulkDeleteRepository.Received(1).DeleteMany(cods);
    }

    [Fact]
    public async Task GetAllAsync_DelegatesToInjectedRepository()
    {
        var repo = CreateRepository();

        await ((IReadRepository<COD>)repo).GetAllAsync(CancellationToken.None);

        await _readRepository.Received(1).GetAllAsync(CancellationToken.None);
    }

    [Fact]
    public void InsertMany_DelegatesToInjectedRepository()
    {
        var cods = new List<COD> { new() { Id = Guid.NewGuid(), NameCOD = "a" } };
        var repo = CreateRepository();

        ((IBulkInsertRepository<COD>)repo).InsertMany(cods);

        _bulkInsertRepository.Received(1).InsertMany(cods);
    }
}

public class TfPlanRepositoryTests
{
    private readonly IManyQueryRepository<TfPlan> _manyQueryRepository = Substitute.For<IManyQueryRepository<TfPlan>>();
    private readonly ICountRepository<TfPlan> _countRepository = Substitute.For<ICountRepository<TfPlan>>();
    private readonly IReadRepository<TfPlan> _readRepository = Substitute.For<IReadRepository<TfPlan>>();
    private readonly IBulkDeleteRepository<TfPlan> _bulkDeleteRepository = Substitute.For<IBulkDeleteRepository<TfPlan>>();
    private readonly IBulkInsertRepository<TfPlan> _bulkInsertRepository = Substitute.For<IBulkInsertRepository<TfPlan>>();

    TfPlanRepository CreateRepository()
        => new(_manyQueryRepository, _countRepository, _readRepository, _bulkDeleteRepository, _bulkInsertRepository);

    [Fact]
    public void InsertMany_DelegatesToInjectedRepository()
    {
        var plans = new List<TfPlan> { new() { Id = Guid.NewGuid(), NameTfPlan = "a" } };
        var repo = CreateRepository();

        ((IBulkInsertRepository<TfPlan>)repo).InsertMany(plans);

        _bulkInsertRepository.Received(1).InsertMany(plans);
    }

    [Fact]
    public void DeleteMany_DelegatesToInjectedRepository()
    {
        var plans = new List<TfPlan> { new() { Id = Guid.NewGuid(), NameTfPlan = "a" } };
        var repo = CreateRepository();

        ((IBulkDeleteRepository<TfPlan>)repo).DeleteMany(plans);

        _bulkDeleteRepository.Received(1).DeleteMany(plans);
    }

    [Fact]
    public async Task GetAllAsync_DelegatesToInjectedRepository()
    {
        var repo = CreateRepository();

        await ((IReadRepository<TfPlan>)repo).GetAllAsync(CancellationToken.None);

        await _readRepository.Received(1).GetAllAsync(CancellationToken.None);
    }

    [Fact]
    public async Task GetCountAsync_DelegatesToInjectedRepository()
    {
        var spec = new GenericSpecification<TfPlan>();
        var repo = CreateRepository();

        await ((ICountRepository<TfPlan>)repo).GetCountAsync(spec, CancellationToken.None);

        await _countRepository.Received(1).GetCountAsync(spec, CancellationToken.None);
    }

    [Fact]
    public async Task GetManyShortAsync_DelegatesToInjectedRepository()
    {
        var spec = new GenericSpecification<TfPlan>();
        var repo = CreateRepository();

        await ((IManyQueryRepository<TfPlan>)repo).GetManyShortAsync(spec, CancellationToken.None);

        await _manyQueryRepository.Received(1).GetManyShortAsync(spec, CancellationToken.None);
    }
}

public class SPRVlansRepositoryTests
{
    private readonly IManyQueryRepository<SPRVlan> _manyQueryRepository = Substitute.For<IManyQueryRepository<SPRVlan>>();
    private readonly ICountRepository<SPRVlan> _countRepository = Substitute.For<ICountRepository<SPRVlan>>();
    private readonly IReadRepository<SPRVlan> _readRepository = Substitute.For<IReadRepository<SPRVlan>>();
    private readonly IBulkDeleteRepository<SPRVlan> _bulkDeleteRepository = Substitute.For<IBulkDeleteRepository<SPRVlan>>();
    private readonly IBulkInsertRepository<SPRVlan> _bulkInsertRepository = Substitute.For<IBulkInsertRepository<SPRVlan>>();

    SPRVlansRepository CreateRepository()
        => new(_manyQueryRepository, _countRepository, _readRepository, _bulkDeleteRepository, _bulkInsertRepository);

    [Fact]
    public void InsertMany_DelegatesToInjectedRepository()
    {
        var vlans = new List<SPRVlan> { new() { Id = Guid.NewGuid(), IdVlan = 1 } };
        var repo = CreateRepository();

        ((IBulkInsertRepository<SPRVlan>)repo).InsertMany(vlans);

        _bulkInsertRepository.Received(1).InsertMany(vlans);
    }

    [Fact]
    public void DeleteMany_DelegatesToInjectedRepository()
    {
        var vlans = new List<SPRVlan> { new() { Id = Guid.NewGuid(), IdVlan = 1 } };
        var repo = CreateRepository();

        ((IBulkDeleteRepository<SPRVlan>)repo).DeleteMany(vlans);

        _bulkDeleteRepository.Received(1).DeleteMany(vlans);
    }

    [Fact]
    public async Task GetAllAsync_DelegatesToInjectedRepository()
    {
        var repo = CreateRepository();

        await ((IReadRepository<SPRVlan>)repo).GetAllAsync(CancellationToken.None);

        await _readRepository.Received(1).GetAllAsync(CancellationToken.None);
    }

    [Fact]
    public async Task GetCountAsync_DelegatesToInjectedRepository()
    {
        var spec = new GenericSpecification<SPRVlan>();
        var repo = CreateRepository();

        await ((ICountRepository<SPRVlan>)repo).GetCountAsync(spec, CancellationToken.None);

        await _countRepository.Received(1).GetCountAsync(spec, CancellationToken.None);
    }

    [Fact]
    public async Task GetManyShortAsync_DelegatesToInjectedRepository()
    {
        var spec = new GenericSpecification<SPRVlan>();
        var repo = CreateRepository();

        await ((IManyQueryRepository<SPRVlan>)repo).GetManyShortAsync(spec, CancellationToken.None);

        await _manyQueryRepository.Received(1).GetManyShortAsync(spec, CancellationToken.None);
    }
}
