namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.LocBillRep;

internal class SPRVlansRepository(IManyQueryRepository<SPRVlan> manyQueryRepository,
                                  ICountRepository<SPRVlan> countRepository,
                                  IReadRepository<SPRVlan> readRepository,
                                  IBulkDeleteRepository<SPRVlan> bulkDeleteRepository,
                                  IBulkInsertRepository<SPRVlan> bulkInsertRepository)
  : ISPRVlansRepository
{

  void IBulkInsertRepository<SPRVlan>.InsertMany(IEnumerable<SPRVlan> entities)
    => bulkInsertRepository.InsertMany(entities);

  void IBulkDeleteRepository<SPRVlan>.DeleteMany(IEnumerable<SPRVlan> entities)
    => bulkDeleteRepository.DeleteMany(entities);

  async Task<IEnumerable<SPRVlan>> IReadRepository<SPRVlan>.GetAllAsync(CancellationToken cancellationToken)
    => await readRepository.GetAllAsync(cancellationToken);

  async Task<int> ICountRepository<SPRVlan>.GetCountAsync(ISpecification<SPRVlan> specification,
                                                          CancellationToken cancellationToken)
    => await countRepository.GetCountAsync(specification,
                                           cancellationToken);

  async Task<IEnumerable<SPRVlan>> IManyQueryRepository<SPRVlan>.GetManyShortAsync(ISpecification<SPRVlan> specification,
                                                                                   CancellationToken cancellationToken)
    => await manyQueryRepository.GetManyShortAsync(specification,
                                                   cancellationToken);
}
