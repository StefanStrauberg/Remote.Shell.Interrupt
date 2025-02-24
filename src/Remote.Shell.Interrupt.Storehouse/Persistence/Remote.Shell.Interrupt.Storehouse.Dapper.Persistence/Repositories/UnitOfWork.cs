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
  IClientCODRRepository IUnitOfWork.ClientCODRs
    => new ClientCODRRepository(_mysqlContext);
  IClientCODLRepository IUnitOfWork.ClientCODLs
    => new ClientCODLRepository(_postgreSQLDapperContext);
  ICODRRepository IUnitOfWork.CODRs
    => new CODRRepository(_mysqlContext);
  ICODLRepository IUnitOfWork.CODLs
    => new CODLRepository(_postgreSQLDapperContext);
  ITfPlanRRepository IUnitOfWork.TfPlanRs
    => new TfPlanRRepository(_mysqlContext);
  ITfPlanLRepository IUnitOfWork.TfPlanLs
    => new TfPlanLRepository(_postgreSQLDapperContext);
  ISPRVlanRsRepository IUnitOfWork.SPRVlanRs
    => new SPRVlanRRepository(_mysqlContext);
  ISPRVlanLsRepository IUnitOfWork.SPRVlanLs
    => new SPRVlanLsRepository(_postgreSQLDapperContext);
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
