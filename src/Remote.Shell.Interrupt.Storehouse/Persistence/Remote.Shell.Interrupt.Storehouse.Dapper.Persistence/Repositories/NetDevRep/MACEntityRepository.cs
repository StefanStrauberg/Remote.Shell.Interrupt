namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.NetDevRep;

internal class MACEntityRepository(IBulkInsertRepository<MACEntity> bulkInsertRepository)
    : IMACEntityRepository
{
  void IBulkInsertRepository<MACEntity>.InsertMany(IEnumerable<MACEntity> entities)
    => bulkInsertRepository.InsertMany(entities);
}