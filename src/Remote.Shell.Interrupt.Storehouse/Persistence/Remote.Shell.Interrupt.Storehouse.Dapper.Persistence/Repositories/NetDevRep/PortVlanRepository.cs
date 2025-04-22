namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.NetDevRep;

internal class PortVlanRepository(IBulkInsertRepository<PortVlan> bulkInsertRepository)
    : IPortVlanRepository
{
  void IBulkInsertRepository<PortVlan>.InsertMany(IEnumerable<PortVlan> entities)
    => bulkInsertRepository.InsertMany(entities);
}
