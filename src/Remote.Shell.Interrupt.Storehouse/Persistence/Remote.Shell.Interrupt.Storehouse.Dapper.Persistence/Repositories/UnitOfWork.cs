namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class UnitOfWork(PostgreSQLDapperContext postgreSQLDapperContext, MySQLDapperContext mySQLDapperContext) : IUnitOfWork, IDisposable
{
  readonly PostgreSQLDapperContext _postgreSQLDapperContext = postgreSQLDapperContext
    ?? throw new ArgumentNullException(nameof(postgreSQLDapperContext));
  readonly MySQLDapperContext _mysqlContext = mySQLDapperContext
    ?? throw new ArgumentNullException(nameof(mySQLDapperContext));
  bool disposed = false;

  INetworkDeviceRepository IUnitOfWork.NetworkDevices
    => new NetworkDeviceRepository(_postgreSQLDapperContext);
  IVLANRepository IUnitOfWork.VLANs
    => new VLANRepository(_postgreSQLDapperContext);
  IPortRepository IUnitOfWork.Ports
    => new PortRepository(_postgreSQLDapperContext);
  IARPEntityRepository IUnitOfWork.ARPEntities
    => new ARPEntityRepository(_postgreSQLDapperContext);
  IMACEntityRepository IUnitOfWork.MACEntities
    => new MACEntityRepository(_postgreSQLDapperContext);
  ITerminatedNetworkEntityRepository IUnitOfWork.TerminatedNetworkEntities
    => new TerminatedNetworkEntityRepository(_postgreSQLDapperContext);
  IPortVlanRepository IUnitOfWork.PortVlans
    => new PortVlanRepository(_postgreSQLDapperContext);
  IRemoteClientsRepository IUnitOfWork.RemoteClients
    => new RemoteClientsRepository(_mysqlContext);
  IClientsRepository IUnitOfWork.Clients
    => new ClientsRepository(_postgreSQLDapperContext);
  IRemoteCODRepository IUnitOfWork.RemoteCODs
    => new RemoteCODRepository(_mysqlContext);
  ICODRepository IUnitOfWork.CODs
    => new CODRepository(_postgreSQLDapperContext);
  IRemoteTfPlanRepository IUnitOfWork.RemoteTfPlans
    => new RemoteTfPlanRepository(_mysqlContext);
  ITfPlanRepository IUnitOfWork.TfPlans
    => new TfPlanRepository(_postgreSQLDapperContext);
  IRemoteSPRVlansRepository IUnitOfWork.RemoteSPRVlans
    => new RemoteSPRVlansRepository(_mysqlContext);
  ISPRVlansRepository IUnitOfWork.SPRVlans
    => new SPRVlansRepository(_postgreSQLDapperContext);
  IGateRepository IUnitOfWork.GateRepository
    => new GateRepository(_postgreSQLDapperContext);

  void IUnitOfWork.Complete()
  {
    _postgreSQLDapperContext.CompleteTransaction();
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
        ((IDisposable)_postgreSQLDapperContext).Dispose();

      disposed = true;
    }
  }

  ~UnitOfWork()
  {
    Dispose(false);
  }
}
