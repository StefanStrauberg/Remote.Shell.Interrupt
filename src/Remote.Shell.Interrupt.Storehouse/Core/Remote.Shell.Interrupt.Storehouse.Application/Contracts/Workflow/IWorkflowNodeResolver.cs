namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Workflow;

/// <summary>
/// Resolves the <see cref="IWorkflowNode"/> implementation registered for a node's technical type.
/// </summary>
public interface IWorkflowNodeResolver
{
  IWorkflowNode Resolve(string type);
}
