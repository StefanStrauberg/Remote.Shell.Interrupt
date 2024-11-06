
namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class UnitOfWork(DapperContext context) : IUnitOfWork, IDisposable
{
  readonly DapperContext _context = context
    ?? throw new ArgumentNullException(nameof(context));
  bool _disposed;

  public IAssignmentRepository Assignments => new AssignmentRepository(context);

  public IBusinessRuleRepository BusinessRules => new BusinessRuleRepository(context);

  public INetworkDeviceRepository NetworkDevices => new NetworkDeviceRepository(context);

  public IVLANRepository VLANs => throw new NotImplementedException();

  public Task CompleteAsync(CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  protected void Dispose(bool disposing)
  {
    if (!_disposed)
    {
      if (disposing)
        _context.Dispose();

      _disposed = true;
    }
  }

  ~UnitOfWork()
  {
    Dispose(false);
  }
}
