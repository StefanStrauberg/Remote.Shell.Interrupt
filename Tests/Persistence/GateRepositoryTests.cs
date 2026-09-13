using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.IGateRep;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.IGateRep;
using Remote.Shell.Interrupt.Storehouse.Specification.Specifications;

namespace Tests.Persistence;

public class GateRepositoryTests
{
    private readonly IExistenceQueryRepository<Gate> _existenceQueryRepository = Substitute.For<IExistenceQueryRepository<Gate>>();
    private readonly ICountRepository<Gate> _countRepository = Substitute.For<ICountRepository<Gate>>();
    private readonly IManyQueryRepository<Gate> _manyQueryRepository = Substitute.For<IManyQueryRepository<Gate>>();
    private readonly IOneQueryRepository<Gate> _oneQueryRepository = Substitute.For<IOneQueryRepository<Gate>>();
    private readonly IInsertRepository<Gate> _insertRepository = Substitute.For<IInsertRepository<Gate>>();
    private readonly IDeleteRepository<Gate> _deleteRepository = Substitute.For<IDeleteRepository<Gate>>();
    private readonly IReplaceRepository<Gate> _replaceRepository = Substitute.For<IReplaceRepository<Gate>>();

    GateRepository CreateRepository()
        => new(_existenceQueryRepository,
               _countRepository,
               _manyQueryRepository,
               _oneQueryRepository,
               _insertRepository,
               _deleteRepository,
               _replaceRepository);

    [Fact]
    public async Task GetOneShortAsync_DelegatesToInjectedRepository()
    {
        var spec = new GenericSpecification<Gate>();
        var repo = CreateRepository();

        await ((IOneQueryRepository<Gate>)repo).GetOneShortAsync(spec, CancellationToken.None);

        await _oneQueryRepository.Received(1).GetOneShortAsync(spec, CancellationToken.None);
    }

    [Fact]
    public async Task AnyByQueryAsync_DelegatesToInjectedRepository()
    {
        var spec = new GenericSpecification<Gate>();
        var repo = CreateRepository();

        await ((IExistenceQueryRepository<Gate>)repo).AnyByQueryAsync(spec, CancellationToken.None);

        await _existenceQueryRepository.Received(1).AnyByQueryAsync(spec, CancellationToken.None);
    }

    [Fact]
    public async Task GetCountAsync_DelegatesToInjectedRepository()
    {
        var spec = new GenericSpecification<Gate>();
        var repo = CreateRepository();

        await ((ICountRepository<Gate>)repo).GetCountAsync(spec, CancellationToken.None);

        await _countRepository.Received(1).GetCountAsync(spec, CancellationToken.None);
    }

    [Fact]
    public async Task GetManyShortAsync_DelegatesToInjectedRepository()
    {
        var spec = new GenericSpecification<Gate>();
        var repo = CreateRepository();

        await ((IManyQueryRepository<Gate>)repo).GetManyShortAsync(spec, CancellationToken.None);

        await _manyQueryRepository.Received(1).GetManyShortAsync(spec, CancellationToken.None);
    }

    [Fact]
    public void InsertOne_DelegatesToInjectedRepository()
    {
        var gate = new Gate { Id = Guid.NewGuid(), Name = "gw" };
        var repo = CreateRepository();

        ((IInsertRepository<Gate>)repo).InsertOne(gate);

        _insertRepository.Received(1).InsertOne(gate);
    }

    [Fact]
    public void DeleteOne_DelegatesToInjectedRepository()
    {
        var gate = new Gate { Id = Guid.NewGuid(), Name = "gw" };
        var repo = CreateRepository();

        ((IDeleteRepository<Gate>)repo).DeleteOne(gate);

        _deleteRepository.Received(1).DeleteOne(gate);
    }

    [Fact]
    public void ReplaceOne_DelegatesToInjectedRepository()
    {
        var gate = new Gate { Id = Guid.NewGuid(), Name = "gw" };
        var repo = CreateRepository();

        ((IReplaceRepository<Gate>)repo).ReplaceOne(gate);

        _replaceRepository.Received(1).ReplaceOne(gate);
    }
}
