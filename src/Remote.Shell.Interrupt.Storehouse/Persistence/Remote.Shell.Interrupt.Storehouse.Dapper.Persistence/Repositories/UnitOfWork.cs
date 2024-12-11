namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class UnitOfWork(PostgreSQLDapperContext postgreSQLDapperContext, MySQLDapperContext mySQLDapperContext) : IUnitOfWork, IDisposable
{
  readonly PostgreSQLDapperContext _postgreSQLDapperContext = postgreSQLDapperContext
    ?? throw new ArgumentNullException(nameof(postgreSQLDapperContext));
  readonly MySQLDapperContext _mysqlContext = mySQLDapperContext
    ?? throw new ArgumentNullException(nameof(mySQLDapperContext));
  bool disposed = false;

  public IAssignmentRepository Assignments
  => new AssignmentRepository(_postgreSQLDapperContext);
  public IBusinessRuleRepository BusinessRules
    => new BusinessRuleRepository(_postgreSQLDapperContext);
  public INetworkDeviceRepository NetworkDevices
    => new NetworkDeviceRepository(_postgreSQLDapperContext);
  public IVLANRepository VLANs
    => new VLANRepository(_postgreSQLDapperContext);
  public IPortRepository Ports
    => new PortRepository(_postgreSQLDapperContext);
  public IARPEntityRepository ARPEntities
    => new ARPEntityRepository(_postgreSQLDapperContext);
  public IMACEntityRepository MACEntities
    => new MACEntityRepository(_postgreSQLDapperContext);
  public ITerminatedNetworkEntityRepository TerminatedNetworkEntities
    => new TerminatedNetworkEntityRepository(_postgreSQLDapperContext);
  public IPortVlanRepository PortVlans
    => new PortVlanRepository(_postgreSQLDapperContext);
  public IClientRepository Clients
    => new ClientRepository(_mysqlContext);

  public void Complete()
  {
    _postgreSQLDapperContext.CompleteTransaction();
  }

  public void Dispose()
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
