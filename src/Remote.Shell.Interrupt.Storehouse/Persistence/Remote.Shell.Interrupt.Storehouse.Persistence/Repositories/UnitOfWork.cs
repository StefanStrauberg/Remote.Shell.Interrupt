namespace Remote.Shell.Interrupt.Storehouse.Persistence.Repositories;

internal class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork, IDisposable
{
  readonly ApplicationDbContext _dbContext = dbContext;
  bool disposed = false;

  public IAssignmentRepository Assignments { get; init; } = new AssignmentRepository(dbContext);
  public IBusinessRuleRepository BusinessRules { get; init; } = new BusinessRuleRepository(dbContext);

  public INetworkDeviceRepository NetworkDevices { get; init; } = new NetworkDeviceRepository(dbContext);

  public IVLANRepository VLANs { get; init; } = new VLANRepository(dbContext);

  public async Task CompleteAsync(CancellationToken cancellationToken)
    => await _dbContext.SaveChangesAsync(cancellationToken);

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
        _dbContext.Dispose();

      disposed = true;
    }
  }

  ~UnitOfWork()
  {
    Dispose(false);
  }
}
