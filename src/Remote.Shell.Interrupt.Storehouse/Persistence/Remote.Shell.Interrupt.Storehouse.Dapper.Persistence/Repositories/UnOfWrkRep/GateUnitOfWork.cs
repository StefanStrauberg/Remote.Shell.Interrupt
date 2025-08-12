namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.UnOfWrkRep;

internal class GateUnitOfWork(ApplicationDbContext applicationContext,
                              IExistenceQueryRepository<Gate> existenceQueryRepository,
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

  Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? _transaction;

  void IUnitOfWork.Complete()
  {
    applicationContext.SaveChanges();

    if (_transaction is not null)
      _transaction?.Commit();
  }

  async Task IUnitOfWork.CompleteAsync(CancellationToken cancellationToken)
  {
    await applicationContext.SaveChangesAsync(cancellationToken);

    if (_transaction is not null)
      await _transaction.CommitAsync(cancellationToken);
  }

  void IUnitOfWork.StartTransaction()
  {
    _transaction = applicationContext.Database.BeginTransaction();
  }

  async Task IUnitOfWork.StartTransactionAsync(CancellationToken cancellationToken)
  {
    _transaction = await applicationContext.Database.BeginTransactionAsync(cancellationToken);
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
        applicationContext.Dispose();
      }

      disposed = true;
    }
  }

  ~GateUnitOfWork()
  {
    Dispose(false);
  }
}
