namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.LocBillRep;

internal class TfPlanRepository(IManyQueryRepository<TfPlan> manyQueryRepository,
                                ICountRepository<TfPlan> countRepository,
                                IReadRepository<TfPlan> readRepository) 
    : ITfPlanRepository
{
  async Task<IEnumerable<TfPlan>> IReadRepository<TfPlan>.GetAllAsync(CancellationToken cancellationToken)
    => await readRepository.GetAllAsync(cancellationToken);

  async Task<int> ICountRepository<TfPlan>.GetCountAsync(RequestParameters requestParameters,
                                                         CancellationToken cancellationToken)
    => await countRepository.GetCountAsync(requestParameters,
                                           cancellationToken);

  async Task<IEnumerable<TfPlan>> IManyQueryRepository<TfPlan>.GetManyShortAsync(RequestParameters requestParameters,
                                                                                 CancellationToken cancellationToken)
    => await manyQueryRepository.GetManyShortAsync(requestParameters,
                                                   cancellationToken);
}