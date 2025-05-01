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
  async Task<Gate> IOneQueryRepository<Gate>.GetOneShortAsync(ISpecification<Gate> specification,
                                                              CancellationToken cancellationToken)
    => await oneQueryRepository.GetOneShortAsync(specification,
                                                 cancellationToken);

  async Task<bool> IExistenceQueryRepository<Gate>.AnyByQueryAsync(ISpecification<Gate> specification,
                                                                   CancellationToken cancellationToken)
    => await existenceQueryRepository.AnyByQueryAsync(specification,
                                                      cancellationToken);

  async Task<int> ICountRepository<Gate>.GetCountAsync(ISpecification<Gate> specification,
                                                       CancellationToken cancellationToken)
    => await countRepository.GetCountAsync(specification,
                                           cancellationToken);

  async Task<IEnumerable<Gate>> IManyQueryRepository<Gate>.GetManyShortAsync(ISpecification<Gate> specification,
                                                                             CancellationToken cancellationToken)
    => await manyQueryRepository.GetManyShortAsync(specification,
                                                   cancellationToken);

  void IInsertRepository<Gate>.InsertOne(Gate entity)
    => insertRepository.InsertOne(entity);

  void IDeleteRepository<Gate>.DeleteOne(Gate entity)
    => deleteRepository.DeleteOne(entity);

  void IReplaceRepository<Gate>.ReplaceOne(Gate entity)
    => replaceRepository.ReplaceOne(entity);
}
