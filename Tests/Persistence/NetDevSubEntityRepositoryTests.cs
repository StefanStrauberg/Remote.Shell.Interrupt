using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.NetDevRep;

namespace Tests.Persistence;

public class ARPEntityRepositoryTests
{
    private readonly IBulkInsertRepository<ARPEntity> _bulkInsertRepository = Substitute.For<IBulkInsertRepository<ARPEntity>>();

    [Fact]
    public void InsertMany_DelegatesToInjectedRepository()
    {
        var entities = new List<ARPEntity> { new() { Id = Guid.NewGuid(), MAC = "m", IPAddress = "1.2.3.4" } };
        var repo = new ARPEntityRepository(_bulkInsertRepository);

        ((IBulkInsertRepository<ARPEntity>)repo).InsertMany(entities);

        _bulkInsertRepository.Received(1).InsertMany(entities);
    }
}

public class MACEntityRepositoryTests
{
    private readonly IBulkInsertRepository<MACEntity> _bulkInsertRepository = Substitute.For<IBulkInsertRepository<MACEntity>>();

    [Fact]
    public void InsertMany_DelegatesToInjectedRepository()
    {
        var entities = new List<MACEntity> { new() { Id = Guid.NewGuid(), MACAddress = "m" } };
        var repo = new MACEntityRepository(_bulkInsertRepository);

        ((IBulkInsertRepository<MACEntity>)repo).InsertMany(entities);

        _bulkInsertRepository.Received(1).InsertMany(entities);
    }
}

public class TerminatedNetworkEntityRepositoryTests
{
    private readonly IBulkInsertRepository<TerminatedNetworkEntity> _bulkInsertRepository = Substitute.For<IBulkInsertRepository<TerminatedNetworkEntity>>();

    [Fact]
    public void InsertMany_DelegatesToInjectedRepository()
    {
        var entities = new List<TerminatedNetworkEntity> { new() { Id = Guid.NewGuid() } };
        var repo = new TerminatedNetworkEntityRepository(_bulkInsertRepository);

        ((IBulkInsertRepository<TerminatedNetworkEntity>)repo).InsertMany(entities);

        _bulkInsertRepository.Received(1).InsertMany(entities);
    }
}

public class VLANRepositoryTests
{
    private readonly IBulkInsertRepository<VLAN> _bulkInsertRepository = Substitute.For<IBulkInsertRepository<VLAN>>();

    [Fact]
    public void InsertMany_DelegatesToInjectedRepository()
    {
        var entities = new List<VLAN> { new() { Id = Guid.NewGuid(), VLANTag = 10, VLANName = "v" } };
        var repo = new VLANRepository(_bulkInsertRepository);

        ((IBulkInsertRepository<VLAN>)repo).InsertMany(entities);

        _bulkInsertRepository.Received(1).InsertMany(entities);
    }
}
