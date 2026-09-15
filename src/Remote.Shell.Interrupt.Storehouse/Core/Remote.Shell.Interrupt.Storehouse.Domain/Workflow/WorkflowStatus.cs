namespace Remote.Shell.Interrupt.Storehouse.Domain.Workflow;

/// <summary>
/// Lifecycle of a <see cref="WorkflowDefinition"/>. A <see cref="Published"/> workflow's
/// graph is immutable - see UpdateWorkflowCommandHandler.
/// </summary>
public enum WorkflowStatus
{
  Draft,
  Published,
  Archived
}
