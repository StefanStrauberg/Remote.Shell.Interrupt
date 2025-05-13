namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.LocBillRep;

internal class CODRepository(IBulkDeleteRepository<COD> bulkDeleteRepository,
                             IReadRepository<COD> readRepository,
                             IBulkInsertRepository<COD> bulkInsertRepository)
  : ICODRepository
{
  void IBulkDeleteRepository<COD>.DeleteMany(IEnumerable<COD> entities)
    => bulkDeleteRepository.DeleteMany(entities);

  async Task<IEnumerable<COD>> IReadRepository<COD>.GetAllAsync(CancellationToken cancellationToken)
    => await readRepository.GetAllAsync(cancellationToken);

  void IBulkInsertRepository<COD>.InsertMany(IEnumerable<COD> entities)
    => bulkInsertRepository.InsertMany(entities);
}