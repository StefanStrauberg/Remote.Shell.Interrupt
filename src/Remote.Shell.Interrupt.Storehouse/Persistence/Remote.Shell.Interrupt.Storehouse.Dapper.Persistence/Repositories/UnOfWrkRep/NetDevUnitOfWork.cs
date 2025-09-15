namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.UnOfWrkRep;

internal class NetDevUnitOfWork(ApplicationDbContext applicationContext,
                                IManyQueryRepository<NetworkDevice> networkDeviceManyQueryRepository,
                                IExistenceQueryRepository<NetworkDevice> networkDeviceExistenceQueryRepository,
                                ICountRepository<NetworkDevice> networkDeviceCountRepository,
                                IInsertRepository<NetworkDevice> networkDeviceInsertRepository,
                                IReadRepository<NetworkDevice> networkDeviceReadRepository,
                                IBulkInsertRepository<VLAN> vlanBulkInsertRepository,
                                IExistenceQueryRepository<Port> portExistenceQueryRepository,
                                IOneQueryRepository<Port> portOneQueryRepository,
                                IBulkInsertRepository<Port> portBulkInsertRepository,
                                IBulkDeleteRepository<Port> portBulkDeleteRepository,
                                IBulkReplaceRepository<Port> portBulkReplaceRepository,
                                IBulkInsertRepository<ARPEntity> arpEntityBulkInsertRepository,
                                IBulkInsertRepository<MACEntity> macEntityBulkInsertRepository,
                                IBulkInsertRepository<TerminatedNetworkEntity> terminatedNetworkEntityBulkInsertRepository)
  : INetDevUnitOfWork, IDisposable
{
  public INetworkDeviceRepository NetworkDevices
    => new NetworkDeviceRepository(applicationContext,
                                   networkDeviceManyQueryRepository,
                                   networkDeviceExistenceQueryRepository,
                                   networkDeviceCountRepository,
                                   networkDeviceInsertRepository,
                                   networkDeviceReadRepository);
  public IVLANRepository VLANs
    => new VLANRepository(vlanBulkInsertRepository);
  public IPortRepository Ports
    => new PortRepository(applicationContext,
                          portExistenceQueryRepository,
                          portOneQueryRepository,
                          portBulkInsertRepository,
                          portBulkDeleteRepository,
                          portBulkReplaceRepository);
  public IARPEntityRepository ARPEntities
    => new ARPEntityRepository(arpEntityBulkInsertRepository);
  public IMACEntityRepository MACEntities
    => new MACEntityRepository(macEntityBulkInsertRepository);
  public ITerminatedNetworkEntityRepository TerminatedNetworkEntities
    => new TerminatedNetworkEntityRepository(terminatedNetworkEntityBulkInsertRepository);

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
    => _transaction = applicationContext.Database.BeginTransaction();

  async Task IUnitOfWork.StartTransactionAsync(CancellationToken cancellationToken)
    => _transaction = await applicationContext.Database.BeginTransactionAsync(cancellationToken);

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
        applicationContext.Dispose();

      disposed = true;
    }
  }

  ~NetDevUnitOfWork()
    => Dispose(false);
}
