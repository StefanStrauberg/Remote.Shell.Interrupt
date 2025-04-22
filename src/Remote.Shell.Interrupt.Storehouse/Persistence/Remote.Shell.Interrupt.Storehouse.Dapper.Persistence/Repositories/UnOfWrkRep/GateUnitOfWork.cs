namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.UnOfWrkRep;

internal class GateUnitOfWork(PostgreSQLDapperContext context,
                              ExistenceQueryRepository<Gate> existenceQueryRepository,
                              CountRepository<Gate> countRepository,
                              ManyQueryRepository<Gate> manyQueryRepository,
                              OneQueryRepository<Gate> oneQueryRepository,
                              InsertRepository<Gate> insertRepository,
                              DeleteRepository<Gate> deleteRepository,
                              ReplaceRepository<Gate> replaceRepository) 
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
