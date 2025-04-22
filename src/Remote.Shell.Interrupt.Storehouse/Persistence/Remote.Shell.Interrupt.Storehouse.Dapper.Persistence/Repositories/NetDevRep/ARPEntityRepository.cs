namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.NetDevRep;

internal class ARPEntityRepository(IBulkInsertRepository<ARPEntity> bulkInsertRepository)
    : IARPEntityRepository
{
  void IBulkInsertRepository<ARPEntity>.InsertMany(IEnumerable<ARPEntity> entities)
    => bulkInsertRepository.InsertMany(entities);
}