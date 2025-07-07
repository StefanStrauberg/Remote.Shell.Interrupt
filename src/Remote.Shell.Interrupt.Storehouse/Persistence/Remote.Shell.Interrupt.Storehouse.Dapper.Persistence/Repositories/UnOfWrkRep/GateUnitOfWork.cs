namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.UnOfWrkRep;

internal class GateUnitOfWork(IExistenceQueryRepository<Gate> existenceQueryRepository,
                              ICountRepository<Gate> countRepository,
                              IManyQueryRepository<Gate> manyQueryRepository,
                              IOneQueryRepository<Gate> oneQueryRepository,
                              IInsertRepository<Gate> insertRepository,
                              IDeleteRepository<Gate> deleteRepository,
                              IReplaceRepository<Gate> replaceRepository) 
  : IGateUnitOfWork, IDisposable
{
  IGateRepository IGateUnitOfWork.Gates 
    => new GateRepository(existenceQueryRepository,
                          countRepository,
                          manyQueryRepository,
                          oneQueryRepository,
                          insertRepository,
                          deleteRepository,
                          replaceRepository);
  
  bool disposed = false;

  void IUnitOfWork.Complete()
  {
    // TODO CompleteTransaction
  }

  void IDisposable.Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!disposed)
    {
      if (disposing)
      {
        // TODO CompleteTransaction
      }

      disposed = true;
    }
  }

  ~GateUnitOfWork()
  {
    Dispose(false);
  }
}
