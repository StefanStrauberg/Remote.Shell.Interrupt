namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.UnOfWrkRep;

internal class RemBillUnitOfWork(MySQLDapperContext context) 
  : IRemBillUnitOfWork, IDisposable
{
  public IRemoteClientsRepository RemoteClients 
    => new RemoteClientsRepository(context);
  public IRemoteCODRepository RemoteCODs 
    => new RemoteCODRepository(context);
  public IRemoteTfPlanRepository RemoteTfPlans 
    => new RemoteTfPlanRepository(context);
  public IRemoteSPRVlansRepository RemoteSPRVlans 
    => new RemoteSPRVlansRepository(context);

  bool disposed = false;

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

  ~RemBillUnitOfWork()
  {
    Dispose(false);
  }
}
