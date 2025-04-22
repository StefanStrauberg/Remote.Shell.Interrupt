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

  async Task<int> ICountRepository<SPRVlan>.GetCountAsync(RequestParameters requestParameters,
                                                          CancellationToken cancellationToken)
    => await countRepository.GetCountAsync(requestParameters,
                                           cancellationToken);

  async Task<IEnumerable<SPRVlan>> IManyQueryRepository<SPRVlan>.GetManyShortAsync(RequestParameters requestParameters,
                                                                                   CancellationToken cancellationToken)
    => await manyQueryRepository.GetManyShortAsync(requestParameters,
                                                   cancellationToken);
}
