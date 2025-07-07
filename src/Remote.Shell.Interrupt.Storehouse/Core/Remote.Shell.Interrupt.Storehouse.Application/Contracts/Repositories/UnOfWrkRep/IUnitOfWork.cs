namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.UnOfWrkRep;

/// <summary>
/// Defines a general unit of work interface for managing with entities.
/// </summary>
public interface IUnitOfWork
{
  /// <summary>
  /// Commits the current unit of work, persisting changes.
  /// </summary>
  void Complete();
}
