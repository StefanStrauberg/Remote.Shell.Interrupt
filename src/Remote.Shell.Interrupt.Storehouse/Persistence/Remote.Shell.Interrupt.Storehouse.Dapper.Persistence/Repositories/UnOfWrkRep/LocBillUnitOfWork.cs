namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.UnOfWrkRep;

internal class LocBillUnitOfWork(ApplicationDbContext applicationContext,
                                 ICountRepository<Client> clientCountRepository,
                                 IExistenceQueryRepository<Client> clientExistenceQueryRepository,
                                 IManyQueryRepository<Client> clientManyQueryRepository,
                                 IReadRepository<Client> clientReadRepository,
                                 IBulkDeleteRepository<Client> clientBulkDeleteRepository,
                                 IBulkInsertRepository<Client> clientBulkInsertRepository,
                                 IBulkDeleteRepository<COD> codBulkDeleteRepository,
                                 IReadRepository<COD> codReadRepository,
                                 IBulkInsertRepository<COD> codBulkInsertRepository,
                                 IManyQueryRepository<TfPlan> tfPlanManyQueryRepository,
                                 ICountRepository<TfPlan> tfPlanCountRepository,
                                 IReadRepository<TfPlan> tfPlanReadRepository,
                                 IBulkDeleteRepository<TfPlan> tfPlanBulkDeleteRepository,
                                 IBulkInsertRepository<TfPlan> tfPlanBulkInsertRepository,
                                 IManyQueryRepository<SPRVlan> sprVlanManyQueryRepository,
                                 ICountRepository<SPRVlan> sprVlanCountRepository,
                                 IReadRepository<SPRVlan> sprVlanReadRepository,
                                 IBulkDeleteRepository<SPRVlan> sprVlanBulkDeleteRepository,
                                 IBulkInsertRepository<SPRVlan> sprVlanBulkInsertRepository) 
  : ILocBillUnitOfWork, IDisposable
{
  IClientsRepository ILocBillUnitOfWork.Clients 
    => new ClientsRepository(applicationContext,
                             clientCountRepository,
                             clientExistenceQueryRepository,
                             clientManyQueryRepository,
                             clientReadRepository,
                             clientBulkDeleteRepository,
                             clientBulkInsertRepository);
  ICODRepository ILocBillUnitOfWork.CODs 
    => new CODRepository(codBulkDeleteRepository,
                         codReadRepository,
                         codBulkInsertRepository);
  ITfPlanRepository ILocBillUnitOfWork.TfPlans 
    => new TfPlanRepository(tfPlanManyQueryRepository,
                            tfPlanCountRepository,
                            tfPlanReadRepository,
                            tfPlanBulkDeleteRepository,
                            tfPlanBulkInsertRepository);
  ISPRVlansRepository ILocBillUnitOfWork.SPRVlans 
    => new SPRVlansRepository(sprVlanManyQueryRepository,
                              sprVlanCountRepository,
                              sprVlanReadRepository,
                              sprVlanBulkDeleteRepository,
                              sprVlanBulkInsertRepository);

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

  ~LocBillUnitOfWork()
  {
    Dispose(false);
  }
}
