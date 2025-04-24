namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.IGateRep;

internal class GateRepository(IExistenceQueryRepository<Gate> existenceQueryRepository,
                              ICountRepository<Gate> countRepository,
                              IManyQueryRepository<Gate> manyQueryRepository,
                              IOneQueryRepository<Gate> oneQueryRepository,
                              IInsertRepository<Gate> insertRepository,
                              IDeleteRepository<Gate> deleteRepository,
                              IReplaceRepository<Gate> replaceRepository)
    : IGateRepository
{
  public Task<Gate> GetOneShortAsync(RequestParameters requestParameters,
                                     CancellationToken cancellationToken)
    => oneQueryRepository.GetOneShortAsync(requestParameters,
                                           cancellationToken);

  void IInsertRepository<Gate>.InsertOne(Gate entity)
    => insertRepository.InsertOne(entity);

  async Task<bool> IExistenceQueryRepository<Gate>.AnyByQueryAsync(RequestParameters requestParameters,
                                                                   CancellationToken cancellationToken)
    => await existenceQueryRepository.AnyByQueryAsync(requestParameters,
                                                      cancellationToken);

  async Task<int> ICountRepository<Gate>.GetCountAsync(RequestParameters requestParameters,
                                                       CancellationToken cancellationToken)
    => await countRepository.GetCountAsync(requestParameters,
                                           cancellationToken);

  async Task<IEnumerable<Gate>> IManyQueryRepository<Gate>.GetManyShortAsync(RequestParameters requestParameters,
                                                                             CancellationToken cancellationToken,
                                                                             bool skipFiltering)
    => await manyQueryRepository.GetManyShortAsync(requestParameters,
                                                   cancellationToken,
                                                   skipFiltering);

  void IDeleteRepository<Gate>.DeleteOne(Gate entity)
    => deleteRepository.DeleteOne(entity);

  void IReplaceRepository<Gate>.ReplaceOne(Gate entity)
    => replaceRepository.ReplaceOne(entity);
}
