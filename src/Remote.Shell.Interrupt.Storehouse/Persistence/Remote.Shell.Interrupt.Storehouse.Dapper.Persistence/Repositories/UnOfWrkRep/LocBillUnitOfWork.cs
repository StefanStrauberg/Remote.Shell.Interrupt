namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.UnOfWrkRep;

internal class LocBillUnitOfWork(PostgreSQLDapperContext context,
                                 CountRepository<Client> clientCountRepository,
                                 ExistenceQueryRepository<Client> clientExistenceQueryRepository,
                                 ManyQueryRepository<Client> clientManyQueryRepository,
                                 ReadRepository<Client> clientReadRepository,
                                 BulkDeleteRepository<Client> clientBulkDeleteRepository,
                                 BulkInsertRepository<Client> clientBulkInsertRepository,
                                 BulkDeleteRepository<COD> codBulkDeleteRepository,
                                 ReadRepository<COD> codReadRepository,
                                 BulkInsertRepository<COD> codBulkInsertRepository,
                                 ManyQueryRepository<TfPlan> tfPlanManyQueryRepository,
                                 CountRepository<TfPlan> tfPlanCountRepository,
                                 ReadRepository<TfPlan> tfPlanReadRepository,
                                 BulkDeleteRepository<TfPlan> tfPlanBulkDeleteRepository,
                                 BulkInsertRepository<TfPlan> tfPlanBulkInsertRepository,
                                 ManyQueryRepository<SPRVlan> sprVlanManyQueryRepository,
                                 CountRepository<SPRVlan> sprVlanCountRepository,
                                 ReadRepository<SPRVlan> sprVlanReadRepository,
                                 BulkDeleteRepository<SPRVlan> sprVlanBulkDeleteRepository,
                                 BulkInsertRepository<SPRVlan> sprVlanBulkInsertRepository) 
  : ILocBillUnitOfWork, IDisposable
{
  IClientsRepository ILocBillUnitOfWork.Clients 
    => new ClientsRepository(context,
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

  void ILocBillUnitOfWork.Complete()
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

  ~LocBillUnitOfWork()
  {
    Dispose(false);
  }
}
