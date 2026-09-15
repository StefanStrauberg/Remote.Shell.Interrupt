namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Workflow;

/// <summary>
/// Runs a <see cref="WorkflowDefinition"/> from its start node to an End node (or to the
/// first failure), routing between nodes via <see cref="EdgeDefinition"/> priority/condition
/// matching. Has no vendor-specific knowledge - that lives entirely in the registered
/// <see cref="IWorkflowNode"/> executors.
/// </summary>
public interface IWorkflowEngine
{
  Task<WorkflowExecutionResult> ExecuteAsync(WorkflowDefinition workflow,
                                             WorkflowContext context,
                                             CancellationToken cancellationToken);
}
