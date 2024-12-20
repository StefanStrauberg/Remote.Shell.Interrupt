using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class UnitOfWork(PostgreSQLDapperContext postgreSQLDapperContext, MySQLDapperContext mySQLDapperContext) : IUnitOfWork, IDisposable
{
  readonly PostgreSQLDapperContext _postgreSQLDapperContext = postgreSQLDapperContext
    ?? throw new ArgumentNullException(nameof(postgreSQLDapperContext));
  readonly MySQLDapperContext _mysqlContext = mySQLDapperContext
    ?? throw new ArgumentNullException(nameof(mySQLDapperContext));
  bool disposed = false;

  IAssignmentRepository IUnitOfWork.Assignments
  => new AssignmentRepository(_postgreSQLDapperContext);
  IBusinessRuleRepository IUnitOfWork.BusinessRules
    => new BusinessRuleRepository(_postgreSQLDapperContext);
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
  IClientRepository IUnitOfWork.Clients
    => new ClientRepository(_mysqlContext);
  IOrganizationsRepository IUnitOfWork.Organizations
    => new OrganizationsRepository(_postgreSQLDapperContext);

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
