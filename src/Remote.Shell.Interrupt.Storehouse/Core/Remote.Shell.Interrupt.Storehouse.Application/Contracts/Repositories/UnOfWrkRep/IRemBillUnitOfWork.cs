namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.UnOfWrkRep;

public interface IRemBillUnitOfWork
{
  IRemoteClientsRepository RemoteClients { get; }
  IRemoteCODRepository RemoteCODs { get; }
  IRemoteTfPlanRepository RemoteTfPlans { get; }
  IRemoteSPRVlansRepository RemoteSPRVlans { get; }
}