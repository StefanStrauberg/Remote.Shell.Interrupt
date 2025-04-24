namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.LocBillRep;

internal class TfPlanRepository(IManyQueryRepository<TfPlan> manyQueryRepository,
                                ICountRepository<TfPlan> countRepository,
                                IReadRepository<TfPlan> readRepository,
                                IBulkDeleteRepository<TfPlan> bulkDeleteRepository,
                                IBulkInsertRepository<TfPlan> bulkInsertRepository)
    : ITfPlanRepository
{
  void IBulkInsertRepository<TfPlan>.InsertMany(IEnumerable<TfPlan> entities)
    => bulkInsertRepository.InsertMany(entities);

  void IBulkDeleteRepository<TfPlan>.DeleteMany(IEnumerable<TfPlan> entities)
    => bulkDeleteRepository.DeleteMany(entities);

  async Task<IEnumerable<TfPlan>> IReadRepository<TfPlan>.GetAllAsync(CancellationToken cancellationToken)
    => await readRepository.GetAllAsync(cancellationToken);

  async Task<int> ICountRepository<TfPlan>.GetCountAsync(RequestParameters requestParameters,
                                                         CancellationToken cancellationToken)
    => await countRepository.GetCountAsync(requestParameters,
                                           cancellationToken);

  async Task<IEnumerable<TfPlan>> IManyQueryRepository<TfPlan>.GetManyShortAsync(RequestParameters requestParameters,
                                                                                 CancellationToken cancellationToken,
                                                                                 bool skipFiltering)
    => await manyQueryRepository.GetManyShortAsync(requestParameters,
                                                   cancellationToken,
                                                   skipFiltering);
}