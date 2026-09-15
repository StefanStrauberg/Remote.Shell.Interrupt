namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.UnOfWrkRep;

/// <summary>
/// Defines a unit of work interface for managing WorkflowDefinition repository operations.
/// </summary>
public interface IWorkflowUnitOfWork : IUnitOfWork
{
  /// <summary>
  /// Gets the repository for WorkflowDefinition entities.
  /// </summary>
  IWorkflowDefinitionRepository Workflows { get; }
}
