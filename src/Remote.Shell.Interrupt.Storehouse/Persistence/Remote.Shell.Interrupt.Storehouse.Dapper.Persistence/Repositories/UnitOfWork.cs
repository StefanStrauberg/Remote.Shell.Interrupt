namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class UnitOfWork(DapperContext context) : IUnitOfWork, IDisposable
{
  readonly DapperContext _context = context
    ?? throw new ArgumentNullException(nameof(context));
  bool disposed = false;

  public IAssignmentRepository Assignments
  => new AssignmentRepository(context);
  public IBusinessRuleRepository BusinessRules
    => new BusinessRuleRepository(context);
  public INetworkDeviceRepository NetworkDevices
    => new NetworkDeviceRepository(context);
  public IVLANRepository VLANs
    => new VLANRepository(context);
  public IPortRepository Ports
    => new PortRepository(context);
  public IARPEntityRepository ARPEntities
    => new ARPEntityRepository(context);
  public IMACEntityRepository MACEntities
    => new MACEntityRepository(context);
  public ITerminatedNetworkEntityRepository TerminatedNetworkEntities
    => new TerminatedNetworkEntityRepository(context);

  public void Complete()
  {
    _context.CompleteTransaction();
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
        _context.Dispose();

      disposed = true;
    }
  }

  ~UnitOfWork()
  {
    Dispose(false);
  }
}
