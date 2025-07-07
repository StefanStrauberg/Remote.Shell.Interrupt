namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.UnOfWrkRep;

/// <summary>
/// Defines a unit of work interface for managing Gate repository operations.
/// </summary>
public interface IGateUnitOfWork : IUnitOfWork
{
  /// <summary>
  /// Gets the repository for Gate entities.
  /// </summary>
  IGateRepository Gates { get; }
}
