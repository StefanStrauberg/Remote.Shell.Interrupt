namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.UnOfWrkRep;

internal class NetDevUnitOfWork(PostgreSQLDapperContext context,
                                ManyQueryRepository<NetworkDevice> networkDeviceManyQueryRepository,
                                ExistenceQueryRepository<NetworkDevice> networkDeviceExistenceQueryRepository,
                                CountRepository<NetworkDevice> networkDeviceCountRepository,
                                InsertRepository<NetworkDevice> networkDeviceInsertRepository,
                                ReadRepository<NetworkDevice> networkDeviceReadRepository,
                                BulkInsertRepository<VLAN> vlanBulkInsertRepository,
                                ExistenceQueryRepository<Port> portExistenceQueryRepository,
                                OneQueryRepository<Port> portOneQueryRepository,
                                BulkInsertRepository<Port> portBulkInsertRepository,
                                BulkDeleteRepository<Port> portBulkDeleteRepository, 
                                BulkReplaceRepository<Port> portBulkReplaceRepository,
                                BulkInsertRepository<ARPEntity> arpEntityBulkInsertRepository,
                                BulkInsertRepository<MACEntity> macEntityBulkInsertRepository,
                                BulkInsertRepository<TerminatedNetworkEntity> terminatedNetworkEntityBulkInsertRepository,
                                BulkInsertRepository<PortVlan> portVlanBulkInsertRepository) 
  : INetDevUnitOfWork, IDisposable
{
  public INetworkDeviceRepository NetworkDevices 
    => new NetworkDeviceRepository(context,
                                   networkDeviceManyQueryRepository,
                                   networkDeviceExistenceQueryRepository,
                                   networkDeviceCountRepository,
                                   networkDeviceInsertRepository,
                                   networkDeviceReadRepository);
  public IVLANRepository VLANs 
    => new VLANRepository(vlanBulkInsertRepository);
  public IPortRepository Ports 
    => new PortRepository(context,
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
  public IPortVlanRepository PortVlans 
    => new PortVlanRepository(portVlanBulkInsertRepository);

  bool disposed = false;

  void INetDevUnitOfWork.Complete()
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

  ~NetDevUnitOfWork()
  {
    Dispose(false);
  }
}
