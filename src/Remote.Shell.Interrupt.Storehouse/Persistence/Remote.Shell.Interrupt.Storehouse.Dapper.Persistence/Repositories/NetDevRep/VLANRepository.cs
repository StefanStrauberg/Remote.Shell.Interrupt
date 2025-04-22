namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.NetDevRep;

internal class VLANRepository(IBulkInsertRepository<VLAN> bulkInsertRepository)
    : IVLANRepository
{
  void IBulkInsertRepository<VLAN>.InsertMany(IEnumerable<VLAN> entities)
    => bulkInsertRepository.InsertMany(entities);
}
