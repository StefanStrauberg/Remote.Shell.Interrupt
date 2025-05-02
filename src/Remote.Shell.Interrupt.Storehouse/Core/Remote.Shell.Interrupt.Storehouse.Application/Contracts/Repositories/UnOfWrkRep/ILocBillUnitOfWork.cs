namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.UnOfWrkRep;

/// <summary>
/// Defines a unit of work interface for managing repository operations related to local billing.
/// </summary>
public interface ILocBillUnitOfWork
{
  /// <summary>
  /// Gets the repository for client entities.
  /// </summary>
  IClientsRepository Clients { get; }

  /// <summary>
  /// Gets the repository for COD entities.
  /// </summary>
  ICODRepository CODs { get; }

  /// <summary>
  /// Gets the repository for TfPlan entities.
  /// </summary>
  ITfPlanRepository TfPlans { get; }

  /// <summary>
  /// Gets the repository for SPRVlan entities.
  /// </summary>
  ISPRVlansRepository SPRVlans { get; }

  /// <summary>
  /// Commits the current unit of work, persisting changes.
  /// </summary>
  void Complete();
}