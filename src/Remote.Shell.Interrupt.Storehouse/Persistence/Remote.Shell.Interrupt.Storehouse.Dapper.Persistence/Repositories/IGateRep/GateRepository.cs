namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.IGateRep;

internal class GateRepository(IExistenceQueryRepository<Gate> existenceQueryRepository,
                              ICountRepository<Gate> countRepository,
                              IManyQueryRepository<Gate> manyQueryRepository,
                              IOneQueryRepository<Gate> oneQueryRepository,
                              IWriteOneRepository<Gate> writeOneRepository)
    : IGateRepository
{
  void IWriteOneRepository<Gate>.DeleteOne(Gate entity)
    => writeOneRepository.DeleteOne(entity);

  void IWriteOneRepository<Gate>.InsertOne(Gate entity)
    => writeOneRepository.InsertOne(entity);

  void IWriteOneRepository<Gate>.ReplaceOne(Gate entity)
    => writeOneRepository.ReplaceOne(entity);

  public Task<Gate> GetOneShortAsync(RequestParameters requestParameters,
                                     CancellationToken cancellationToken)
    => oneQueryRepository.GetOneShortAsync(requestParameters,
                                           cancellationToken);

  async Task<bool> IExistenceQueryRepository<Gate>.AnyByQueryAsync(RequestParameters requestParameters,
                                                                   CancellationToken cancellationToken)
    => await existenceQueryRepository.AnyByQueryAsync(requestParameters,
                                                      cancellationToken);

  async Task<int> ICountRepository<Gate>.GetCountAsync(RequestParameters requestParameters,
                                                       CancellationToken cancellationToken)
    => await countRepository.GetCountAsync(requestParameters,
                                           cancellationToken);

  async Task<IEnumerable<Gate>> IManyQueryRepository<Gate>.GetManyShortAsync(RequestParameters requestParameters,
                                                                             CancellationToken cancellationToken)
    => await manyQueryRepository.GetManyShortAsync(requestParameters,
                                                   cancellationToken);
}
