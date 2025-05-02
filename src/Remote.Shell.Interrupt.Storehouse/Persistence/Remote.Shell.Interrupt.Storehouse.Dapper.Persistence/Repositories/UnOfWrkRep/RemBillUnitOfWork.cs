namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.UnOfWrkRep;

internal class RemBillUnitOfWork(MySQLDapperContext context,
                                 IAppLogger<RemoteClientsRepository> remoteClientsRepositoryAppLogger,
                                 IAppLogger<RemoteCODRepository> remoteCODRepositoryAppLogger,
                                 IAppLogger<RemoteTfPlanRepository> remoteTfPlanRepositoryAppLogger,
                                 IAppLogger<RemoteSPRVlansRepository> remoteSPRVlansRepositoryAppLogger) 
  : IRemBillUnitOfWork, IDisposable
{
  public IRemoteClientsRepository RemoteClients 
    => new RemoteClientsRepository(context,
                                   remoteClientsRepositoryAppLogger);
  public IRemoteCODRepository RemoteCODs 
    => new RemoteCODRepository(context,
                               remoteCODRepositoryAppLogger);
  public IRemoteTfPlanRepository RemoteTfPlans 
    => new RemoteTfPlanRepository(context,
                                  remoteTfPlanRepositoryAppLogger);
  public IRemoteSPRVlansRepository RemoteSPRVlans 
    => new RemoteSPRVlansRepository(context,
                                    remoteSPRVlansRepositoryAppLogger);

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
