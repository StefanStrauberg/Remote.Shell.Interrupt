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

  public Task CompleteAsync(CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
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
