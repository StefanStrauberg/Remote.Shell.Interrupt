namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.NetDevRep;

internal class TerminatedNetworkEntityRepository(IBulkInsertRepository<TerminatedNetworkEntity> bulkInsertRepository)
    : ITerminatedNetworkEntityRepository
{
  void IBulkInsertRepository<TerminatedNetworkEntity>.InsertMany(IEnumerable<TerminatedNetworkEntity> entities)
    => bulkInsertRepository.InsertMany(entities); 
}