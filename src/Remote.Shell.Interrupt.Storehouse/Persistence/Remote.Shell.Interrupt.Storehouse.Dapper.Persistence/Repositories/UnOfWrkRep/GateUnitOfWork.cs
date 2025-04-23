namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.UnOfWrkRep;

internal class GateUnitOfWork(PostgreSQLDapperContext context,
                              IExistenceQueryRepository<Gate> existenceQueryRepository,
                              ICountRepository<Gate> countRepository,
                              IManyQueryRepository<Gate> manyQueryRepository,
                              IOneQueryRepository<Gate> oneQueryRepository,
                              IInsertRepository<Gate> insertRepository,
                              IDeleteRepository<Gate> deleteRepository,
                              IReplaceRepository<Gate> replaceRepository) 
  : IGateUnitOfWork, IDisposable
{
  IGateRepository IGateUnitOfWork.GateRepository 
    => new GateRepository(existenceQueryRepository,
                          countRepository,
                          manyQueryRepository,
                          oneQueryRepository,
                          insertRepository,
                          deleteRepository,
                          replaceRepository);
  
  bool disposed = false;

  void IGateUnitOfWork.Complete()
  {
    context.CompleteTransaction();
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
        ((IDisposable)context).Dispose();

      disposed = true;
    }
  }

  ~GateUnitOfWork()
  {
    Dispose(false);
  }
}
