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
        // TODO Dispose context
      }

      disposed = true;
    }
  }

  ~LocBillUnitOfWork()
  {
    Dispose(false);
  }
}
