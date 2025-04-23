namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.UnOfWrkRep;

internal class NetDevUnitOfWork(PostgreSQLDapperContext context,
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
                                IBulkInsertRepository<TerminatedNetworkEntity> terminatedNetworkEntityBulkInsertRepository,
                                IBulkInsertRepository<PortVlan> portVlanBulkInsertRepository) 
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
