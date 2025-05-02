namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.UnOfWrkRep;

/// <summary>
/// Defines a unit of work interface for managing remote billing-related repository operations.
/// </summary>
public interface IRemBillUnitOfWork
{
  /// <summary>
  /// Gets the repository for remote client entities.
  /// </summary>
  IRemoteClientsRepository RemoteClients { get; }

  /// <summary>
  /// Gets the repository for remote COD entities.
  /// </summary>
  IRemoteCODRepository RemoteCODs { get; }

  /// <summary>
  /// Gets the repository for remote TfPlan entities.
  /// </summary>
  IRemoteTfPlanRepository RemoteTfPlans { get; }

  /// <summary>
  /// Gets the repository for remote SPRVlan entities.
  /// </summary>
  IRemoteSPRVlansRepository RemoteSPRVlans { get; }
}